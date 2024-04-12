using ChatHubApi.Services;
using ChatHubApi.System;
using ChatHubApi.System.Entity.Font;
using construct.Application.System.Services.Login.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace construct.Application.System.Services.Login
{
    /// <summary>
    /// 授权服务[前台]
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly ISqlSugarClient _db;
        private readonly IConfiguration _config;
        private readonly JwtSecurityTokenHandler _jwtHandler;

        public AuthController(ISqlSugarClient db, IConfiguration config, JwtSecurityTokenHandler jwtHandler)
        {
            _db = db;
            _config = config;
            _jwtHandler = jwtHandler;
        }



        /// <summary>
        /// 登陆授权
        /// </summary>
        /// <param name="loginInput"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> login([FromBody]FontLoginInput loginInput)
        {
            sysFontUser user = await _db.Queryable<sysFontUser>().SingleAsync(a => a.Username == loginInput.userName && a.Password==loginInput.passworld);
            if (user == null) return Ok(new Response(code:2,message: "用户名或密码错误！", data:new object()));
            

            string accessToken = string.Empty;

            if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
            {
                // 生成 token
                var claims = new List<Claim>
                {
                    new Claim("UserId", user.id.ToString()),
                    new Claim("UserName", user.Username),
                    new Claim("Role","User"),
                };
                var expiresAt = DateTime.UtcNow.AddMinutes(300);
                accessToken = jwtService.CreateJwtToken(claims, expiresAt, _jwtHandler, _config);
               
            }

            var returnMsg = new
            {
                userId = user.id,
                userName = user.Username,
                userPsw = user.Password,
                jwtToken = accessToken,
                userImg = user.HeaderImg,
            };
            return Ok(new Response(
                code:1,
                data:returnMsg,
                message:"登录成功"));
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="loginInput"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public async Task<IActionResult> register([FromBody] FontLoginInput loginInput)
        {
            var Exist =await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == loginInput.userName);
            if(Exist != null) {
                return Ok(new Response(code: 3, message: "已经存在此用户！", data: new object()));
            }
            sysFontUser user = new sysFontUser()
            {
                Username = loginInput.userName,
                Password = loginInput.passworld,
                HeaderImg = "head1.svg",
                Desc = "这个人还没有填写介绍",
                status= ChatHubApi.System.Enum.Status.DISABLE,
            };
            string accessToken = string.Empty;
            if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
            {
                // 生成 token
                var claims = new List<Claim>
                {
                    new Claim("UserId", user.id.ToString()),
                    new Claim("UserName", user.Username),
                    new Claim("Role","User"),
                };
                var expiresAt = DateTime.UtcNow.AddMinutes(300);
                accessToken = jwtService.CreateJwtToken(claims, expiresAt, _jwtHandler, _config);

            }
            try
            {
                _db.Insertable(user).ExecuteCommand();
                //初始化分组系统
                sysRelationTree tree = new sysRelationTree();
                int id  = await _db.Queryable<sysFontUser>().Where(x=>x.Username == user.Username ).Select(x=>x.id).FirstAsync();
                tree.ownerId = id;
                tree.name = "好友";
                _db.Insertable(tree).ExecuteCommand();
                var returnMsg = new
                {
                    userId = id,
                    userName = user.Username,
                    userPsw = user.Password,
                    jwtToken = accessToken,
                    userImg = user.HeaderImg,
                };
                return Ok(new Response(
                code: 1,
                data: returnMsg,
                message: "注册成功"));
            }
            catch(Exception ex)
            {
                return Ok(new Response(
                    code: 2,
                    data: null,
                    message: "系统异常" + ex.Message));
            }

        }




        [Authorize]
        [HttpGet]
        public ActionResult Verify()
        {
            return Ok(new Response(code: 1, new object(), string.Empty));
        }

        [HttpPost]
        public ActionResult AES()
        {
            
            return Ok(new Response(code: 1, new object(), string.Empty));
        }
    }
}
