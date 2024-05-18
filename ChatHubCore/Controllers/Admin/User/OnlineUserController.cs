using ChatHubApi.Controllers.AdminServices.User.Model;
using ChatHubApi.System;
using ChatHubApi.System.Entity.Font;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace ChatHubApi.Controllers.AdminServices.User
{
    /// <summary>
    /// 在线用户表 服务
    /// </summary>
    [ApiController]
    [Route("admin/[controller]/[action]")]
    public class OnlineUserController:ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public OnlineUserController(ISqlSugarClient db)
        {
            _db = db;
        }



        /// <summary>
        /// 获取所有在线用户       
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] OnlineUserSearchModel? md)
        {
            if (md != null)
            {
                var query = _db.Queryable<sysOnlineUser>()
                .WhereIF(!string.IsNullOrEmpty(md.name), it => it.name.Contains(md.name));
                var users = await query.ToListAsync();

                return Ok(new Response(1, users, "搜索成功"));
            }
            var res = _db.Queryable<sysOnlineUser>().ToList();
            return Ok(new Response(1, res, "获取成功"));
        }

        /// <summary>
        /// 通过Username模糊查询       
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult queryByUserName(string keyword)
        {
            var res = _db.Queryable<sysOnlineUser>().Where(it => it.name.Contains(keyword)).ToList();
            if (res == null) return NotFound();
            StringBuilder json = new StringBuilder();
            foreach (var item in res)
            {
                json.Append(JsonSerializer.Serialize(item, new JsonSerializerOptions()
                {
                    // 整齐打印
                    WriteIndented = true,
                    //重新编码，解决中文乱码问题
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                }) + "$");
            }

            return Ok(json.ToString());
        }

    }

}
