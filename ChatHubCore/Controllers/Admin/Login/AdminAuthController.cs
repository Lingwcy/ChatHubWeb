using ChatHubApi.Controllers.AdminServices.Login.Model;
using ChatHubApi.Services;
using ChatHubApi.System.Entity;
using ChatHubApi.Untils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AdminAuthController> _logger;

        public AdminAuthController(ISqlSugarClient db, IConfiguration config, JwtSecurityTokenHandler jwtHandler, ILogger<AdminAuthController> logger)
        {
            _db = db;
            _config = config;
            _jwtHandler = jwtHandler;
            _logger = logger;
        }
        [HttpPost]
        public async Task<ActionResult<string>> login(LoginInput loginInput)
        {
            if (loginInput.account == null || loginInput.psw == null)
            {
                return BadRequest(new { code = 0, message = "请输入用户名和密码" });
            }

            // 查询用户
            var admin = await _db.Queryable<sysAdmin>().FirstAsync(it => it.name == loginInput.account);
            if (admin == null)
            {
                return BadRequest(new { code = 0, message = "用户名或密码错误" });
            }

            // 验证密码哈希
            if (!Crypto.VerifyHashedPassword(admin.psw, loginInput.psw))
            {
                _logger.LogWarning("Admin login failed: {Account}", loginInput.account);
                return BadRequest(new { code = 0, message = "用户名或密码错误" });
            }

            // 生成 token
            var claims = new List<Claim>
            {
                new Claim("UserId", loginInput.account.ToString()),
                new Claim("Role", "admin"),
            };
            var expiresAt = DateTime.UtcNow.AddMinutes(120);
            var output = jwtService.CreateJwtToken(claims, expiresAt, _jwtHandler, _config);

            return Ok(new { code = 1, data = new { token = output }, message = "登录成功" });
        }
    }
}
