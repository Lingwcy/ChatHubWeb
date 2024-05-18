using ChatHubApi.Controllers.AdminServices.User;
using ChatHubApi.Controllers.AdminServices.User.Model;
using ChatHubApi.System.Entity.Font;
using ChatHubApi.System;
using Microsoft.AspNetCore.Http;
using ChatHubApi.Controllers.Admin.Friend;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace ChatHubApi.Controllers.Admin.Friend
{
    /// <summary>
    /// 好友服务 ADMIN API
    /// </summary>
    [ApiController]
    [Route("admin/[controller]/[action]")]
    public class FriendAdminController : ControllerBase
    {

        private readonly ISqlSugarClient _db;
        private readonly ILogger<UserController> _logger;

        public FriendAdminController(ISqlSugarClient db, ILogger<UserController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有好友关系
        /// version 2.0
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetFriendInputModel? md)
        {
            if (md.userId!= null)
            {
                var query = _db.Queryable<sysFriends>()
                .Where(it => it.TheUserId.ToString() == md.userId);
                var users = await query.ToListAsync();

                return Ok(new Response(1, users, "搜索成功"));
            }
            var res = _db.Queryable<sysFriends>().ToList();
            return Ok(new Response(1, res, "获取成功"));
        }



        /// <summary>
        /// 获取所有好友请求
        /// version 2.0
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public async Task<IActionResult> GetAllRequests([FromQuery] GetFriendRequestInputModel? md)
        {
            if (md.userId != null)
            {
                var query = _db.Queryable<sysFriendsRequest>()
                .Where(it => it.UserId.ToString() == md.userId);
                var users = await query.ToListAsync();

                return Ok(new Response(1, users, "搜索成功"));
            }
            var res = _db.Queryable<sysFriendsRequest>().ToList();
            return Ok(new Response(1, res, "获取成功"));
        }
    }
}
