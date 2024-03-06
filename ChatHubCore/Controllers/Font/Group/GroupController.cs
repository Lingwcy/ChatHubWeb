using ChatHubApi.System;
using ChatHubApi.System.Entity.Font;
using construct.Application.System.Services.Login.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.IdentityModel.Tokens.Jwt;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace ChatHubApi.Controllers.Font.Group
{
    /// <summary>
    /// 群组服务[前台]
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class GroupController : ControllerBase
    {
        private readonly ISqlSugarClient _db;
        private readonly IConfiguration _config;
        private readonly JwtSecurityTokenHandler _jwtHandler;
        private readonly ILogger<GroupController> _logger;

        public GroupController(ISqlSugarClient db, IConfiguration config, JwtSecurityTokenHandler jwtHandler, ILogger<GroupController> logger)
        {
            _db = db;
            _config = config;
            _jwtHandler = jwtHandler;
            _logger = logger;
        }

        /// <summary>
        /// 查询群组通过ID
        /// (Anyone)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetById(int id = 0)
        {
            //如果为0 查找所有群组
            if(id == 0) {
                return Ok(new Response(1, _db.Queryable<sysGroups>().ToList(), "操作成功!"));
            }
            else
            {
                return Ok(new Response(1, _db.Queryable<sysGroups>().First(e => e.GroupId == id), "操作成功!"));
            }
        }

        /// <summary>
        /// 查询群组通过Name
        /// (Anyone)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetByName(string groupName)
        {
            if(groupName == string.Empty) { return Ok(new Response(2, null, "错误的格式")); }
            var query = _db.Queryable<sysGroups>()
            .WhereIF(!string.IsNullOrEmpty(groupName), it => it.GroupName.Contains(groupName))
                           .ToList();

            if(query.Count > 0)
            {
                return Ok(new Response(1, query, "查询成功"));
            }
            else
            {
                return Ok(new Response(3, query, "没有相符合的群组"));
            }
        }

        /// <summary>
        /// 查询一个群组内的所有成员
        /// (MemberOnly)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Members(int groupId)
        {
            if(_db.Queryable<sysGroups>().Any(e => e.GroupId != groupId)) 
            { return Ok(new Response(3, null, "找不到此群组!")); }
            var idList = _db.Queryable<sysGroups>()
                .Mapper(it => it.UserGroup, it => it.GroupId)
                .First(x => x.GroupId == groupId)
                .UserGroup
                .Select(x => x.UserId);
            List<object> memberList = new List<object>();
            foreach(var id in idList)
            {
                var res = await _db.Queryable<sysFontUser>()
                     .FirstAsync(x => x.id == id);
                memberList.Add(new {id= id,name =res.Username});           
            }
            return Ok(new Response(1, memberList, "操作成功!"));
        }


        /// <summary>
        /// 加入一个群组
        /// (SelfOnly)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Join([FromBody]Model.JoinModel md)
        {
            //判断是否存在此用户
            if (_db.Queryable<sysFontUser>().Any(x => x.id != md.UserId))
            {
                return Ok(new Response(3, null, "找不到此用户!"));
            }
            //判断是否存在此群
            if (_db.Queryable<sysGroups>().Any(x => x.GroupId != md.GroupId))
            {
                return Ok(new Response(3, null, "找不到此群!"));
            }
            //判断用户是否已经加入此群
            if (_db.Queryable<sysUserGroup>().Any(x => x.UserId == md.UserId && x.UserGroupId == x.UserGroupId))
            {
                return Ok(new Response(2, null, "你已经是群成员了！"));
            }

            sysUserGroup sysUserGroup = new sysUserGroup()
            {
                UserId = md.UserId,
                GroupId = md.GroupId,
                JoinDate = DateTime.Now,
                IsActive = true,
            };
            var i = _db.Insertable(sysUserGroup).ExecuteCommand();
            return Ok(new Response(1, i > 0, "操作成功！"));
        }
    }
}
