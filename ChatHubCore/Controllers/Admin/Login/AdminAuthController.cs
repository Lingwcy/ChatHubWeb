using ChatHubApi.Controllers.AdminServices.Login.Model;
using ChatHubApi.Services;
using ChatHubApi.System.Entity;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatHubApi.Controllers.AdminServices.Login
{
    /// <summary>
    /// 授权服务[后台]
    /// 
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminAuthController : ControllerBase
    {

        private readonly ISqlSugarClient _db;
        private readonly IConfiguration _config;
        private readonly JwtSecurityTokenHandler _jwtHandler;

        public AdminAuthController(ISqlSugarClient db, IConfiguration config, JwtSecurityTokenHandler jwtHandler)
        {
            _db = db;
            _config = config;
            _jwtHandler = jwtHandler;
        }
        [HttpPost]
        public async Task<ActionResult<string>> login(LoginInput loginInput)
        {
            if (loginInput.account == null || loginInput.psw == null)
            {
                return NotFound();
            }
            var res = await _db.Queryable<sysAdmin>().FirstAsync(it => it.name == loginInput.account && it.psw == loginInput.psw);
            if (res == null)
            {
                return NotFound();
            }
            string output = string.Empty;


            if (!string.IsNullOrEmpty(loginInput.account) && !string.IsNullOrEmpty(loginInput.psw))
            {
                // 生成 token
                // 生成 token
                var claims = new List<Claim>
                {
                    new Claim("UserId", loginInput.account.ToString()),
                    new Claim("Role","admin"),
                };
                var expiresAt = DateTime.UtcNow.AddMinutes(300);
                output = jwtService.CreateJwtToken(claims, expiresAt, _jwtHandler, _config);
            }

            return NotFound();
        }
    }
}
