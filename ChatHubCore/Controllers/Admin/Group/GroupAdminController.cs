using ChatHubApi.Controllers.Admin.Friend;
using ChatHubApi.Controllers.AdminServices.User;
using ChatHubApi.System.Entity.Font;
using ChatHubApi.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace ChatHubApi.Controllers.Admin.Group
{
    /// <summary>
    /// 群组服务 ADMIN API
    /// </summary>
    [ApiController]
    [Route("admin/[controller]/[action]")]
    public class GroupAdminController : ControllerBase
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogger<UserController> _logger;

        public GroupAdminController(ISqlSugarClient db, ILogger<UserController> logger)
        {
            _db = db;
            _logger = logger;
        }
        /// <summary>
        /// 获取所有群组
        /// version 2.0
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetGroupModel? md)
        {
            if (md.name != null)
            {
                var query = _db.Queryable<sysGroups>()
                .WhereIF(!string.IsNullOrEmpty(md.name), it => it.GroupName.Contains(md.name));
                var groups = await query.ToListAsync();

                return Ok(new Response(1, groups, "搜索成功"));
            }
            else
            {
                var res = _db.Queryable<sysGroups>().ToList();
                return Ok(new Response(1, res, "获取成功"));
            }
        }


    }
}
