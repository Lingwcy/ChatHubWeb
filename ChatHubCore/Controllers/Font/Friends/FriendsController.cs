﻿using ChatHubApi.System;
using ChatHubApi.System.Entity.Font;
using construct.Application.System.FontServices.Friends.Model;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace construct.Application.System.FontServices.Friends
{
    /// <summary>
    /// 好友服务(前台)
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class FriendsController : ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public FriendsController(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 查找好友（好友）
        /// 描述:目标表 SysFriends
        /// version 2.0
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Authorize(policy: "SelfOnly")]
        public async Task<ActionResult> Query(string targetName, string username)
        {
            if (targetName == username)
            {
                return Ok(new Response(2, null, "不能添加自己为好友"));
            }

            sysFontUser fUser = await _db.Queryable<sysFontUser>().FirstAsync(a=>a.Username== targetName);
            sysFontUser SUser = await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == username);
            if (fUser == null)
            {
                return Ok(new Response(3, null, "找不到此用户"));
            }
            var repeat = await _db.Queryable<sysFriends>().SingleAsync(a => a.FriendId == fUser.id && a.FriendName == fUser.Username && a.TheUserId == SUser.id);
            if (repeat != null)
            {
                return Ok(new Response(4, null, "已经是好友了"));
            }

            return Ok(new Response(1, null, "查找成功！")); ;//成功
        }


        /// <summary>
        /// 获取一个用户的所有好友
        /// version: 2.0
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Authorize(policy: "SelfOnly")]
        public async Task<ActionResult> QueryAll(long userId)
        {
            sysFontUser user =await _db.Queryable<sysFontUser>().FirstAsync(a=>a.id== userId);
            if (user == null)
            {
                return NotFound();
            }
            var res = _db.Queryable<sysFriends>().Where(a => a.TheUserId == userId).ToList();
            return Ok(new Response(1, res, "获取成功"));
        }


        /// <summary>
        /// 发送一个好友请求(向请求表插入数据)
        ///  version: 2.0
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="username"></param>
        /// <param name="ReqMsg"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Authorize(policy: "SelfOnly")]
        public async Task<IActionResult> SendRequest([FromBody] sendRequestModel md)
        {
            sysFontUser TUser = await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == md.targetName);
            sysFontUser SUser = await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == md.userName);
            if (TUser == null || SUser == null)
            {
                return Ok(new Response(3,null,"不存在此用户！"));
            }
            var repeat = await _db.Queryable<sysFriendsRequest>().FirstAsync(a => a.UserId == SUser.id && a.TargetId == TUser.id);
            if (repeat != null)
            {
                return Ok(new Response(2, null, "已经发送过请求了！"));
            }

            sysFriendsRequest friendsReq = new sysFriendsRequest() { ReqMsg = md.ReqMsg, UserId = SUser.id, TargetId = TUser.id, TargetName = TUser.Username, UserName = SUser.Username, TargetImg = TUser.HeaderImg, UserImg = SUser.HeaderImg, remark = md.mark };
            return Ok(new Response(1, _db.Insertable<sysFriendsRequest>(friendsReq).ExecuteCommand(), "发送请求成功！")); ;

        }


        /// <summary>
        /// 获取好友请求列表
        /// version: 2.0
        /// </summary>
        /// <param name="username">用户名称</param>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Authorize(policy:"SelfOnly")]
        public new async Task<IActionResult> Request(string username)
        {
            StringBuilder sb = new StringBuilder();
            sysFontUser SUser =await _db.Queryable<sysFontUser>().FirstAsync(a=>a.Username== username);  
            var reqList = await _db.Queryable<sysFriendsRequest>().Where(a => a.TargetId == SUser.id).ToListAsync();
            return Ok(new Response(code: 1, message: "查找成功!", data: reqList));

        }

        /// <summary>
        /// 拒绝好友请求(删除表中的请求)
        /// version 2.0
        /// </summary>
        /// <param name="friendsReq"></param>
        /// <returns></returns>
        /// 
        [HttpDelete]
        [Authorize(policy: "SelfOnly")]
        public new IActionResult Request([FromQuery]sysFriendsRequest friendsReq)
        {
            sysFriendsRequest res =friendsReq.Adapt<sysFriendsRequest>();

            return Ok(new Response(code: (_db.Deleteable<sysFriendsRequest>().Where(a => a.UserId == res.UserId && a.TargetId == res.TargetId).ExecuteCommand()) > 0 ? 1 : 2 , message: "操作成功!", data: null));
        }

        /// <summary>
        /// 接受好友请求（此过程需删除好友请求表并添加好友关系表）
        /// version 2.0
        /// </summary>
        /// <param name="friendsReq"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Authorize(policy: "SelfOnly")]
        public async Task<IActionResult> AcceptRequest([FromBody]sysFriendsRequest friendsReq)
        {
            //创建一个双向好友 =>同时添加两条数据
            sysFriends friends1 = new sysFriends()
            {
                TheUserId = friendsReq.UserId,
                FriendId = friendsReq.TargetId,
                FriendName = friendsReq.TargetName,
                FriendImg = friendsReq.TargetImg,
                remark = friendsReq.remark,
            };
            sysFriends friends2 = new sysFriends()
            {
                TheUserId = friendsReq.TargetId,
                FriendId = friendsReq.UserId,
                FriendName = friendsReq.UserName,
                FriendImg = friendsReq.UserImg,
                remark = "",
            };

            bool repeat = await _db.Queryable<sysFriends>().AnyAsync(a => a.TheUserId == friendsReq.UserId && a.FriendId == friendsReq.TargetId);
            var res = friendsReq.Adapt<sysFriendsRequest>();
            if (repeat)
            {
                _db.Deleteable<sysFriendsRequest>().Where(a => a.UserId == res.UserId && a.TargetId == res.TargetId).ExecuteCommand();
                return Ok(new Response(code: 3, message: "已经是好友了!", data: null));//已经是好友了
            }
            _db.Insertable<sysFriends>(friends1).ExecuteCommand();
            _db.Insertable<sysFriends>(friends2).ExecuteCommand();
            _db.Deleteable<sysFriendsRequest>().Where(a => a.UserId == res.UserId && a.TargetId == res.TargetId).ExecuteCommand();
            return Ok(new Response(code: 1, message: "添加成功!", data: null));
        }


        /// <summary>
        /// 获取消息盒子列表(同时会获取好友请求)
        /// version 2.0
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public async Task<IActionResult> MessageBox(string username)
        {
            sysFontUser SUser = await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == username);
            var msgBoxList = await _db.Queryable<sysMsgBox>().Where(a => a.username == SUser.Username).ToListAsync();

            return Ok(new Response(1, msgBoxList, "操作成功！"));

        }



        /// <summary>
        /// 已读取消消息盒子冒红
        /// version 2.0
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public async Task<IActionResult> Redbob(sendRedBobModel md)
        {
            var exist = await _db.Queryable<sysMsgBox>().FirstAsync(a => a.username == md.targetname && a.targetfont == md.username);
            if (exist == null)
            {
                return NotFound();
            }

                var result = _db.Updateable<sysMsgBox>()
                .SetColumns(it => new sysMsgBox { isNew = 0 })//类只能在表达示里面不能提取
                .Where(a => a.username == md.username && a.targetfont == md.targetname)
                .ExecuteCommand();
            return Ok(new Response(1, result, "操作成功！"));
        }

    }

}
