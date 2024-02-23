using ChatHubApi.Controllers.AdminServices.Message.Model;
using ChatHubApi.System.Entity.Font;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ChatHubApi.Controllers.AdminServices.Message
{
    /// <summary>
    /// 消息暂存 服务
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminMessageController:ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public AdminMessageController(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 获取所有暂存消息      
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult<string> getAll()
        {
            var res = _db.Queryable<sysMessageRecord>().ToList();
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

            return json.ToString();
        }


        /// <summary>
        /// 增加消息体     
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public async Task<ActionResult<int>> add(AddMessageInput input)
        {
            var exits = await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == input.Sender);
            var exits2 = await _db.Queryable<sysFontUser>().FirstAsync(a => a.Username == input.Receiver);
            if (exits == null || exits2 == null)
            {
                return NotFound();
            }
            string SenderImg = exits.HeaderImg;
            sysMessageRecord mr = new()
            {
                Sender = input.Sender,
                Receiver = input.Receiver,
                SendMessage = input.SendMessage,
                SenderImg = SenderImg,
                CreateTime = DateTime.Now,
            };

            return _db.Insertable<sysMessageRecord>(mr).ExecuteCommand();

        }




        /// <summary>
        /// 编辑消息体       
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        [HttpPut]
        public async Task<ActionResult<int>> update(EditMessageInput input)
        {
            var isExist = await _db.Queryable<sysMessageRecord>().SingleAsync(a => a.id == input.id);
            if (isExist == null) return NotFound();

            sysMessageRecord mr = new sysMessageRecord()
            {
                Sender = input.Sender,
                Receiver = input.Receiver,
                SendMessage = input.SendMessage,
                CreateTime = DateTime.Now,
                id = input.id,
            };

            return _db.Updateable(mr).ExecuteCommand();

        }


        /// <summary>
        /// 删除一个消息体根据ID      
        /// </summary>
        /// <param name="id">删除的id</param>
        /// <returns></returns>
        /// 
        [HttpDelete]
        public async Task<ActionResult<int>> delete(int id)
        {
            var isExist = await _db.Queryable<sysMessageRecord>().SingleAsync(a => a.id == id);
            if (isExist == null) return NotFound();


            return _db.Deleteable<sysMessageRecord>().Where(a => a.id == id).ExecuteCommand();

        }




        /// <summary>
        /// 消息体批量删除（通过传入id数组来搜索）      
        /// </summary>
        /// <param name="dc">需要删除的消息数组</param>
        /// <returns></returns>
        /// 
        [HttpDelete]
        public Task deletes([FromBody] dynamic dc)
        {


            JsonElement json = dc;
            List<int> objs = JsonSerializer.Deserialize<List<int>>(json.ToString());
            foreach (var item in objs)
            {
                _db.Deleteable<sysMessageRecord>().Where(a => a.id == item).ExecuteCommand();
            }

            return Task.CompletedTask;

        }


        /// <summary>
        /// 通过Sender模糊查询       
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult<string> queryBySender(string keyword)
        {
            var res = _db.Queryable<sysMessageRecord>().Where(it => it.Sender.Contains(keyword)).ToList();
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

            return json.ToString();
        }

    }

}
