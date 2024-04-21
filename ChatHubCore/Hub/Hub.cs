using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;
using SqlSugar;
using System.Text;
using ChatHubApi.System.Entity.Font;
using Microsoft.Extensions.Logging;
using ChatHubApi.Untils;
using Microsoft.IdentityModel.Tokens;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ChatHubApi.Hub
{
    public class MyHub : Hub<IHub>
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogger<MyHub> _logger;
        private readonly IConfiguration _configuration;

        public MyHub(ISqlSugarClient db, ILogger<MyHub> logger, IConfiguration configuration)
        {
            _db = db;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// HUB类重写方法 在客户端连接时检测jwt内容 并将用户信息存储在后台的在线表
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var name = Context.User?.Claims.First(a => a.Type == "UserName").Value;
            var id = Context.User?.Claims.First(a => a.Type == "UserId").Value;
            var conId = Context.ConnectionId;
            await _db.Deleteable<sysOnlineUser>().Where(e => e.name == name).ExecuteCommandAsync();
            await _db.Insertable(new sysOnlineUser
            {
                conId = conId,
                name = name,
                userid = id,
                createtime = DateTime.Now,
            }).ExecuteCommandAsync();
            _logger.LogInformation($"[+] 用户：{name} => ID {id}");

            //查找该用户所在的群组，添加到群组
            var groups = await _db.Queryable<sysUserGroup>().Where(a => a.UserId == int.Parse(id)).Select(a => a.GroupId).ToListAsync();
            foreach (var group in groups)
            {
                await Groups.AddToGroupAsync(conId, group.ToString());
                _logger.LogInformation($"[+] 用户：{name} => 加入群组： {group}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            var name = Context.User?.Claims.First(a => a.Type == "UserName").Value;
            var id = Context.User?.Claims.First(a => a.Type == "UserId").Value;
            var conId = Context.ConnectionId;
            await _db.Deleteable<sysOnlineUser>().Where(e => e.name == name && e.conId == conId).ExecuteCommandAsync();
            if (e != null)
            {
                _logger.LogWarning($"[-] 用户：{name} => ID {id} 异常断开 {e.Message}");
            }
            else
            {
                _logger.LogInformation($"[-] 用户：{name} => ID {id}");
            }
            //查找该用户所在的群组，移除用户
            var groups = await _db.Queryable<sysUserGroup>().Where(a => a.UserId == int.Parse(id)).Select(a => a.GroupId).ToListAsync();
            foreach (var group in groups)
            {
                await Groups.RemoveFromGroupAsync(conId, group.ToString());
                _logger.LogInformation($"[-] 用户：{name} => 退出群组： {group}");
            }
        }
        public async Task SendPublicMsg(string FromName, string msg)
        {
            sysFontUser user = await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == FromName);
            await Clients.All.PublicMsgReceived(user.HeaderImg, user.Username, msg);
        }
        public async Task SendGroupMsg(string FromName, string msg,string GroupId)
        {
            /**
             * 
             *  目前由于工作量原因，广播消息并没有进行加密。
             *  构想：
             *  在SysGroups中新增Key字段
             *  群组在创建时由创建者创建一个群组ASE密钥并推送到数据库SysGroups表中
             *  广播消息使用此Key进行加密并发送
             */
            //拿到发送者的key
            string key;
            try
            {
                key = _db.Queryable<sysOnlineUser>().First(x => x.name == FromName).key;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "获取发送者的key失败");
                return;
            }
            //用发送者的key进行解密
            var resmsg = Crypto.DecryptByAES(msg, key, key);
            sysFontUser user = await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == FromName);
            await Clients.OthersInGroup(GroupId).GroupMsgReceived(user.HeaderImg,user.Username, resmsg, GroupId);
        }
        public async Task SendPrivateMsg(string FromName ,string toUserName, string message)
        {
            //拿到发送者的key
            string key = _db.Queryable<sysOnlineUser>().First(x => x.name == FromName).key;
            //用发送者的key进行解密
            var resmsg = Crypto.DecryptByAES(message, key, key);
            _logger.LogError($"{FromName} => {toUserName} : {resmsg}");

           
            //获取发送的客户端
            sysOnlineUser CurrentUser = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.conId == Context.ConnectionId);
            string HeadImg = (await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == CurrentUser.name)).HeaderImg;

            //获取目标客户端(需要判断此用户是否在线  如果不在线就存放到 临时消息存储表=> t-messageRecored)
            var TargetUser = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.name == toUserName);
            if (TargetUser == null)
            {
                //emoji过滤
                foreach (var a in resmsg)
                {
                    byte[] bts = Encoding.UTF32.GetBytes(a.ToString());

                    if (bts[0].ToString() == "253" && bts[1].ToString() == "255")
                    {
                        resmsg = resmsg.Replace(a.ToString(), "");
                    }

                }
                if (resmsg == string.Empty) { return; }
                //(string Key, string Iv)  = Crypto.Get(_configuration);
                //var serverEncryptMessage =Crypto.EncryptByAES(resmsg, Key, Iv);
                sysMessageRecord mr = new() { CreateTime = DateTime.Now, Sender = CurrentUser.name, Receiver = toUserName, SendMessage = resmsg, SenderImg = HeadImg };
                _db.Insertable(mr).ExecuteCommand();
                return;
            }


            //用接收者的key进行加密
            var EncryptMsg = Crypto.EncryptByAES(resmsg, TargetUser.key, TargetUser.key);

            //如果在线就直接发送信息 CurrentUser.name 为发送者
            await Clients.Client(TargetUser.conId).PrivateMsgReceived(HeadImg, CurrentUser.name, EncryptMsg);
        }
        public async Task SendFriendsRequest(string toUserName)
        {
            //获取发送的客户端
            sysOnlineUser CurrentUser = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.conId == Context.ConnectionId);
            var TargetUser = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.name == toUserName);
            if (TargetUser == null)
            {
                return;
            }

            //如果在线就直接发送信息
            await Clients.Client(TargetUser.conId).FriendsRequestReceived(CurrentUser.name);
        }
        public async Task MsgBoxFlasher(string toUserName)
        {
            //获取发送的客户端
            sysOnlineUser CurrentUser = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.conId == Context.ConnectionId);
            string currentImg = (await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == CurrentUser.name)).HeaderImg;

            var TargetUser = await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == toUserName);
            if (TargetUser == null)
            {
                return;
            }
            var exist = await _db.Queryable<sysMsgBox>().FirstAsync(a => a.username == TargetUser.Username && a.targetfont == CurrentUser.name);
            if (exist == null)
            {
                sysMsgBox res = new sysMsgBox()
                {
                    username = TargetUser.Username,
                    targetfont = CurrentUser.name,
                    targetImage = currentImg,
                    isNew = 1,
                    Type ="person"
                };
                _db.Insertable(res).ExecuteCommand();
            }
  
                var result = _db.Updateable<sysMsgBox>()
                .SetColumns(it => new sysMsgBox { isNew = 1 })//类只能在表达示里面不能提取
                .Where(a => a.username == TargetUser.Username && a.targetfont == CurrentUser.name)
                .ExecuteCommand();

            //如果在线就直接发送信息 接到消息 立即刷新目标客户端的msgbox
            var online = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.name == toUserName);
            if (online == null)
            {
                return;
            }
            await Clients.Client(online.conId).MsgBoxFlasherReceived(CurrentUser.name);
        }

        public async Task GroupMsgBoxFlasher(string GroupId,string FromName)
        {
            //获取此群的所有用户
            var users = await _db.Queryable<sysUserGroup>().Where(a => a.GroupId == int.Parse(GroupId)).ToListAsync();
            //获取群名称
            var groupName = (await _db.Queryable<sysGroups>().FirstAsync(a => a.GroupId == int.Parse(GroupId))).GroupName;
            foreach(var user in users){
                //获取用户名
                var fontuser = await _db.Queryable<sysFontUser>().FirstAsync(a => a.id == user.UserId);
                var exist = await _db.Queryable<sysMsgBox>().FirstAsync(a => a.username == fontuser.Username && a.targetfont == groupName);
                if (exist == null)
                {
                    sysMsgBox res = new sysMsgBox()
                    {
                        username = fontuser.Username,
                        targetfont = groupName,
                        targetImage = fontuser.HeaderImg,
                        isNew = 1,
                        Type ="group"
                    };
                    _db.Insertable(res).ExecuteCommand();
                }
   
                    var result = _db.Updateable<sysMsgBox>()
                    .SetColumns(it => new sysMsgBox { isNew = 1 })//类只能在表达示里面不能提取
                    .Where(a => a.username == fontuser.Username && a.targetfont == groupName)
                    .ExecuteCommand();
                //立即刷新群在线成员的客户端msgbox
                await Clients.OthersInGroup(GroupId).GroupMsgBoxFlasherReceived();
            }
        }

        public void SendHubKey(string FromName, string Key)
        {
            var user = _db.Queryable<sysOnlineUser>().First(x => x.name == FromName);
            if (user == null) { Context.Abort(); return; }
            user.key = Key;
            _db.Updateable<sysOnlineUser>(user).Where(x => x.conId == Context.ConnectionId).ExecuteCommand();
        }

        public async Task CreateGroupTask(sysGroups group, int[] userIds,int creatorId)
        {
            //1.将群组内在线的成员添加进signalR的群组列表中
            //2.通知群组的成员刷新群组列表
            userIds = userIds.Append(creatorId).ToArray();
            try
            {
                foreach (var userId in userIds)
                {
                    var user = await _db.Queryable<sysOnlineUser>().FirstAsync(a => int.Parse(a.userid) == userId);
                    if (user == null) { continue; }
                    await Groups.AddToGroupAsync(user.conId, group.GroupId.ToString());
                    _logger.LogInformation($"[+] 用户：{user.id} => 加入群组： {group.GroupName}");
                    await Clients.Client(user.conId).RefreshGroupList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建群组通知任务执行失败");
            }
        }
    }
}
