using ChatHubApi.Controllers.Font.Group.Model;
using ChatHubApi.Services;
using ChatHubApi.System;
using ChatHubApi.System.Entity.Font;
using construct.Application.System.FontServices.Friends.Model;
using construct.Application.System.Services.Login.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            if (id == 0)
            {
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
            if (groupName == string.Empty) { return Ok(new Response(2, null, "错误的格式")); }
            var query = _db.Queryable<sysGroups>()
            .WhereIF(!string.IsNullOrEmpty(groupName), it => it.GroupName.Contains(groupName))
                           .ToList();

            if (query.Count > 0)
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
            var flag = _db.Queryable<sysGroups>().Where(e => e.GroupId == groupId).Count();
            if (flag == 0)
            { return Ok(new Response(3, null, "找不到此群组!")); }
            /***
             * 因为UserGroup的类型是List<sysUserGroup>
             * 此时Mapper方法根据GroupId查询UserGroup，然后将UserGroup的类型与实体sysUserGroup进行映射
             */
            var userList = _db.Queryable<sysGroups>()
                .Mapper(it => it.UserGroup, it => it.GroupId)
                .First(x => x.GroupId == groupId)
                .UserGroup
                .Select(x => (x.UserId, x.Role));
            List<object> memberList = new List<object>();
            foreach (var user in userList)
            {
                var res = await _db.Queryable<sysFontUser>()
                     .FirstAsync(x => x.id == user.UserId);
                memberList.Add(new { id = res.id, name = res.Username, Role = user.Role });
            }
            return Ok(new Response(1, memberList, "操作成功!"));
        }


        /// <summary>
        /// 加入一个群组
        /// (SelfOnly)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Join([FromBody] Model.JoinModel md)
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

        /// <summary>
        /// 查询自己加入的群组
        /// (SelfOnly)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult MyGroups(int userId)
        {
            var id = _db.Queryable<sysFontUser>().Where(x => x.id == userId).Select(x => x.id).First();
            if (id == 0)
            {
                return Ok(new Response(3, null, "找不到此用户!"));
            }

            var query = _db.Queryable<sysUserGroup>()
                .Where(x => x.UserId == id)
                .Mapper(it => it.Group, it => it.GroupId)
                .ToList();

            return Ok(new Response(1, query, "操作成功!"));
        }


        /// <summary>
        /// 加群(发送群组请求)
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public IActionResult SendRequest([FromBody] SendGroupRequestModel md)
        {
            //不存在此群组
            var group = _db.Queryable<sysGroups>().First(x => x.GroupId == md.GroupId);
            if (group == null)
            {
                return Ok(new Response(3, null, "找不到此群组!"));
            }
            //已经加入了此群组
            var joined = _db.Queryable<sysUserGroup>().Where(x => x.GroupId == md.GroupId && x.UserId == md.UserId).First();
            if (joined != null)
            {
                return Ok(new Response(3, null, "你已经加入了此群组!"));
            }
            //已经发过此请求了
            var existRequest = _db.Queryable<sysGroupReuqest>().First(x => x.UserId == md.UserId && x.TargetGroupId == md.GroupId);
            if (existRequest != null)
            {
                return Ok(new Response(3, null, "你已经发送过了此请求!"));
            }

            //创建请求
            sysGroupReuqest request = new sysGroupReuqest()
            {
                UserId = md.UserId,
                TargetGroupId = md.GroupId,
                ReqMsg = md.ReqMsg,
            };
            var i = _db.Insertable(request).ExecuteCommand();
            if (i > 0)
            {
                return Ok(new Response(1, i, "操作成功!"));
            }
            else
            {
                return Ok(new Response(2, i, "操作失败!"));
            }
        }

        /// <summary>
        /// 获取群请求
        /// (SelfOnly)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Requests(int userId)
        {
            var myGroupIds = _db.Queryable<sysGroups>()
                .Where(x => x.CreatorUserId == userId)
                .Select(x => x.GroupId)
                .ToList();
            if (myGroupIds.Count == 0)
            {
                return Ok(new Response(2, null, "你还没有创建任何群组!"));
            }
            List<sysGroupReuqest> myGroupRequests = new List<sysGroupReuqest>();
            foreach (var groupId in myGroupIds)
            {
                var requests = _db.Queryable<sysGroupReuqest>()
                    .Where(x => x.TargetGroupId == groupId)
                    .ToList();
                myGroupRequests.AddRange(requests);
            }
            return Ok(new Response(1, myGroupRequests, "操作成功!"));
        }

        /// <summary>
        /// 接受群请求
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public IActionResult AcceptRequest([FromBody] AcceptGroupRequestModel md)
        {
            var request = _db.Queryable<sysGroupReuqest>()
                .Where(x => x.Id == md.Id).First();
            if (request == null)
            {
                return Ok(new Response(3, null, "找不到此请求!"));
            }
            //将此用户添加进群组成员表
            sysUserGroup userGroup = new sysUserGroup()
            {
                UserId = request.UserId,
                GroupId = request.TargetGroupId,
                JoinDate = DateTime.Now,
                Role = "成员",
                IsActive = true,
            };
            var i = _db.Insertable(userGroup).ExecuteCommand();
            if (i > 0)
            {
                //删除请求
                var j = _db.Deleteable<sysGroupReuqest>().Where(x => x.Id == md.Id).ExecuteCommand();
                if (j > 0)
                {
                    return Ok(new Response(1, i, "操作成功!"));
                }
                else
                {
                    return Ok(new Response(2, i, "操作失败!"));
                }
            }
            else
            {
                return Ok(new Response(2, i, "操作失败!"));
            }
        }

        /// <summary>
        /// 拒绝群请求
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpDelete]
        public IActionResult RejectRequest([FromQuery] RejectGroupRequestModel md)
        {
            var request = _db.Queryable<sysGroupReuqest>()
                .Where(x => x.Id == md.Id).First();
            if (request == null)
            {
                return Ok(new Response(3, null, "找不到此请求!"));
            }
            //删除请求
            var i = _db.Deleteable<sysGroupReuqest>().Where(x => x.Id == md.Id).ExecuteCommand();
            if (i > 0)
            {
                return Ok(new Response(1, i, "操作成功!"));
            }
            else
            {
                return Ok(new Response(2, i, "操作失败!"));
            }

        }

        /// <summary>
        /// 创建群组
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public IActionResult Create([FromBody] CreateGroupModel md)
        {
            //先判断所有用户是否存在
            foreach (var userId in md.UserId)
            {
                var exist = _db.Queryable<sysFontUser>().First(x => x.id == userId);
                if (exist == null)
                {
                    return Ok(new Response(3, null, "找不到此用户!"));
                }
            }
            sysGroups group = new sysGroups();
            group.GroupName = md.Name;
            group.CreatorUserId = md.CreateUserId;
            group.CreationDate = DateTime.Now;
            _db.Insertable(group).ExecuteCommand();

            //将所有成员添加进该组
            //BUG 同一个用户不能创建一个重复名称的群组
            var groupRes = _db.Queryable<sysGroups>().First(x => x.GroupName == md.Name && x.CreatorUserId == md.CreateUserId );
            if (groupRes.GroupId == 0)
            {
                return Ok(new Response(2, null, "创建群组失败!"));
            }
            foreach(var userId in md.UserId)
            {
                sysFontUser user = _db.Queryable<sysFontUser>().First(x => x.id == userId);
                sysUserGroup userGroup = new sysUserGroup()
                {
                    GroupId = groupRes.GroupId,
                    UserId = userId,
                    JoinDate = DateTime.Now,
                    Role = "成员",
                    IsActive = true,
                }; 
                _db.Insertable(userGroup).ExecuteCommand();
                //添加成员之后，群人数+1
                groupRes.MemberNumber += 1;
                _db.Updateable(groupRes).ExecuteCommand();
            }
            
            //添加创始人为群主
            sysUserGroup creatorUserGroup = new sysUserGroup()
            {
                GroupId = groupRes.GroupId,
                UserId = md.CreateUserId,
                JoinDate = DateTime.Now,
                Role = "群主",
                IsActive = true,
            };
            _db.Insertable(creatorUserGroup).ExecuteCommand();
            groupRes.MemberNumber += 1;
            _db.Updateable(groupRes).ExecuteCommand();
            return Ok(new Response(1, groupRes, "创建群组成功!"));
        }
    }
}
