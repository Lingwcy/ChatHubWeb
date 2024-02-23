using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;
using SqlSugar;
using System.Text;
using ChatHubApi.System.Entity.Font;

namespace construct.Web.Entry
{
    public class MyHub: Hub
    {
        private readonly ISqlSugarClient _db;
        private readonly JwtSecurityTokenHandler _jwtHandler;

        public MyHub(ISqlSugarClient db,JwtSecurityTokenHandler jwthandler)
        {
            _db = db;
            _jwtHandler = jwthandler;
        }

        /// <summary>
        /// HUB类重写方法 在客户端连接时检测jwt内容 并将用户信息存储在后台的在线表
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {

            var token = Context.GetHttpContext()?.Request.Query["access_token"];
            var claims = _jwtHandler.ReadJwtToken(token).Claims.ToList();
            //var claims = JWTEncryption.ReadJwtToken(token)?.Claims;
            var name = claims.FirstOrDefault(e => e.Type == "UserName")?.Value;
            var id = claims.FirstOrDefault(e => e.Type == "UserId")?.Value;
            var conId = Context.ConnectionId;
            await _db.Deleteable<sysOnlineUser>().Where(e => e.name == name).ExecuteCommandAsync();
            await _db.Insertable<sysOnlineUser>(new sysOnlineUser
            {
                conId= conId,
                name=name,
                userid=id,
                createtime= DateTime.Now,
           }).ExecuteCommandAsync();
            Console.WriteLine($"{name} 加入了");
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            var token = Context.GetHttpContext()?.Request.Query["access_token"];
            var claims = _jwtHandler.ReadJwtToken(token).Claims.ToList();
            var name = claims.FirstOrDefault(e => e.Type == "UserName")?.Value;
            var id = claims.FirstOrDefault(e => e.Type == "UserId")?.Value;
            var conId = Context.ConnectionId;
            Console.WriteLine($"{ name} 离开了");
            await _db.Deleteable<sysOnlineUser>().Where(e => e.name==name && e.conId==conId).ExecuteCommandAsync();
        }


        public async Task SendPublicMsg(string FromName,string msg)
        {
            sysFontUser user =await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == FromName);      
            string ConnId= this.Context.ConnectionId;
           await this.Clients.All.SendAsync("publicMsgReceived",user.HeaderImg, user.Username, msg);
        }
        public async Task SendPrivateMsg(string toUserName,string message)
        {
            //获取发送的客户端
            sysOnlineUser CurrentUser = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.conId == Context.ConnectionId);
            string HeadImg = (await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == CurrentUser.name)).HeaderImg;

            //获取目标客户端(需要判断此用户是否在线  如果不在线就存放到 临时消息存储表=> t-messageRecored)
            var TargetUser = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.name == toUserName);
            if (TargetUser == null)
            {
                //emoji过滤
                foreach (var a in message)
                {
                    byte[] bts = Encoding.UTF32.GetBytes(a.ToString());

                    if (bts[0].ToString() == "253" && bts[1].ToString() == "255")
                    {
                        message = message.Replace(a.ToString(), "");
                    }

                }
                if(message==string.Empty) { return ; }
                sysMessageRecord mr = new() { CreateTime = DateTime.Now, Sender = CurrentUser.name, Receiver = toUserName ,SendMessage=message,SenderImg=HeadImg};
                _db.Insertable<sysMessageRecord>(mr).ExecuteCommand();
                return;
            }

            //如果在线就直接发送信息
            await Clients.Client(TargetUser.conId).SendAsync("PrivateMsgReceived", HeadImg,CurrentUser.name, message);
        }

        public async Task SendFriendsRequest(string toUserName)
        {
            //获取发送的客户端
            sysOnlineUser CurrentUser = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.conId == Context.ConnectionId);
            var TargetUser = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.name == toUserName);
            if(TargetUser==null)
            {
                return;
            }

            //如果在线就直接发送信息
            await Clients.Client(TargetUser.conId).SendAsync("FriendsRequestReceived", CurrentUser.name);
        }


        public async Task MsgBoxFlasher(string toUserName)
        {
            //获取发送的客户端
            sysOnlineUser CurrentUser = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.conId == Context.ConnectionId);
            string currentImg =( await _db.Queryable<sysFontUser>().FirstAsync(a=>a.Username== CurrentUser.name)).HeaderImg;
          
            var TargetUser = await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == toUserName);
            if (TargetUser == null)
            {
                return;
            }
            var exist =await _db.Queryable<sysMsgBox>().FirstAsync(a => a.username == TargetUser.Username && a.targetfont == CurrentUser.name);
            if (exist == null)
            {
                sysMsgBox res = new sysMsgBox()
                {
                    username= TargetUser.Username,
                    targetfont= CurrentUser.name,
                    targetImage= currentImg,
                    isNew=1,
                };
                _db.Insertable(res).ExecuteCommand();
            }
            else
            {
                var result = _db.Updateable<sysMsgBox>()
                .SetColumns(it => new sysMsgBox {isNew=1})//类只能在表达示里面不能提取
                .Where(a => a.username == TargetUser.Username && a.targetfont == CurrentUser.name)
                .ExecuteCommand();
            }
            //如果在线就直接发送信息 接到消息 立即刷新目标客户端的msgbox
            var online = await _db.Queryable<sysOnlineUser>().FirstAsync(a => a.name == toUserName);
            if (online == null)
            {
                return;
            }
            await Clients.Client(online.conId).SendAsync("MsgBoxFlasherReceived", CurrentUser.name);
        }




    }
}
