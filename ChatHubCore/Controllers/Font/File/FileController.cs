using ChatHubApi.Hub;
using ChatHubApi.System;
using ChatHubApi.System.Entity.Font;
using ChatHubApi.System.Enum;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic.FileIO;
using SqlSugar;
using System.Security.Policy;

namespace ChatHubApi.Controllers.Font.File
{
    /// <summary>
    /// 文件服务    
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class FileController : ControllerBase
    {
        readonly IConfiguration _configuration;
        private readonly ISqlSugarClient _db;
        private readonly IHubContext<MyHub, IHub> _hubContext;
        public FileController(IConfiguration configuration, ISqlSugarClient db, IHubContext<MyHub, IHub> hubContext)
        {
            _configuration = configuration;
            _hubContext = hubContext;
            _db = db;
        }

        [HttpPost]
        //[AllowAnonymous]
        public IActionResult Upload([FromBody] UploadFileModel md)
        {
            byte[] imageBytes = Convert.FromBase64String(md.base64String);
            //将imageBytes转化为IFormFile
            var file = new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, md.fileName , md.fileName);
            string guid = Guid.NewGuid().ToString("N");
            string path = $"Files/images/{guid}-{md.fileName}";
            string physicPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            string dir = Path.GetDirectoryName(physicPath);
            if (!Directory.Exists(dir))Directory.CreateDirectory(dir);
            using (FileStream fs = new FileStream(physicPath, FileMode.Create))
            {
                file.CopyTo(fs);
            }
            string url = path;
            var res = new
            {
                name = $"{guid}-{md.fileName}",
                url = path,
            };
            return Ok(new Response(1, res, "上传成功!"));

        }

        [HttpGet]
        public IActionResult GetImage(string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Files/images", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, "image/png"); // 根据图片的实际MIME类型修改  
        }


        [HttpPost]
        public IActionResult UserAvatar([FromBody] UploadAvatarModel md)
        {
            var user = _db.Queryable<sysFontUser>().Where(x => x.id == md.userId).First();
            if (user == null)
            {
                return Ok(new Response(2, null, "用户不存在!"));
            }
            //修改sysfontuser表
            user.HeaderImg = md.img;
            var f1 = _db.Updateable(user).Where(x => x.id == md.userId).ExecuteCommand();
            //修改sysfriends表
            var f2 = _db.Updateable<sysFriends>()
               .SetColumns(it =>it.FriendImg == md.img)
               .Where(it => it.FriendId == md.userId)
               .ExecuteCommand();
            //修改sysMessageRecord
            var f3 = _db.Updateable<sysMessageRecord>()
               .SetColumns(it => it.SenderImg == md.img)
               .Where(it => it.Sender == user.Username)
               .ExecuteCommand();
            //修改msgbox
            var f4 = _db.Updateable<sysMsgBox>()
               .SetColumns(it => it.targetImage == md.img)
               .Where(it => it.targetId == md.userId)
               .ExecuteCommand();

            //发送siganlr消息给该用户的在线的好友修改头像信息(用户的好友执行刷新好友列表数据)

            _db.Queryable<sysFriends>().Where(x => x.FriendId == md.userId).ToList().ForEach(x =>
            {
                //判断每一个好友是否在线
                var online = _db.Queryable<sysOnlineUser>().Where(y => int.Parse(y.userid) == x.TheUserId).First();
                if (online != null)
                {
                    _hubContext.Clients.Client(online.conId).RefreshFriendList();
                }
            });
            if (f1 >= 0 && f2 >= 0 && f3 >= 0 && f4 >= 0)
            {
                return Ok(new Response(1, null, "修改成功!"));
            }
            else
            {
                return Ok(new Response(2, null, "修改失败!"));
            }

        }

        [HttpPost]
        public async Task<IActionResult> UserInfo([FromBody] UploadUserInfoModel md)
        {
            var isExist = await _db.Queryable<sysFontUser>().SingleAsync(a => a.id== md.id);
            if (isExist == null) return Ok(new Response(3, null, "不存在此用户"));

            var res = _db.Updateable<sysFontUser>()
                .SetColumns(it => new sysFontUser {Job = md.Job,Email = md.Email,City = md.City,Sex = md.Sex,Phone = md.Phone,Birth = md.Birth,Desc = md.Desc })
                .Where(it => it.id == md.id).ExecuteCommand();


            //发送siganlr消息给该用户的在线的好友修改头像信息(用户的好友执行刷新好友列表数据)
            _db.Queryable<sysFriends>().Where(x => x.FriendId == md.id).ToList().ForEach(x =>
            {
                //判断每一个好友是否在线
                var online = _db.Queryable<sysOnlineUser>().Where(y => int.Parse(y.userid) == x.TheUserId).First();
                if (online != null)
                {
                    _hubContext.Clients.Client(online.conId).RefreshFriendList();
                }
            });
            return Ok(new Response(res > 0 ? 1 : 2, null, "操作成功"));

        }
    }
}
