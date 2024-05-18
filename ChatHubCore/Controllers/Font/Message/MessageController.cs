
using ChatHubApi.System;
using ChatHubApi.System.Entity.Font;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace construct.Application.System.FontServices.Message
{
    /// <summary>
    /// 信息服务
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class MessageController:ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public MessageController(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 获取离线消息
        /// </summary>
        /// version 2.0
        /// <param name="username">用户名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> offline(string username) {
           var res = await _db.Queryable<sysMessageRecord>().Where(a=>a.Receiver==username).OrderBy(a=>a.CreateTime).ToArrayAsync();
            foreach (var record in res)
            {
                bool existrecord = false;
                var useMsgboxlist = _db.Queryable<sysMsgBox>().Where(a => a.username == username).ToList();
                foreach (var msgbox in useMsgboxlist)
                {
                    if (record.Sender == msgbox.targetfont)
                    {
                        existrecord = true;
                        msgbox.isNew = 1;
                        _db.Updateable<sysMsgBox>()
                       .SetColumns(it => it.isNew == 1)
                       .Where(a => a.username == username && a.targetfont == record.Sender)
                       .ExecuteCommand();
                    }
                }
                if (!existrecord)
                {
                    sysMsgBox mg = new sysMsgBox { username = username, targetfont = record.Sender, targetImage = record.SenderImg, isNew = 1 };
                    _db.Insertable(mg).ExecuteCommand();
                }
            }

             _db.Deleteable<sysMessageRecord>().Where(a => a.Receiver == username).ExecuteCommand();
            return Ok(new Response(1,res,"操作成功!"));
        }




        /// <summary>
        /// 删除一个MsgBox Item
        /// version 2.0
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult messageBoxItem([FromQuery]int id)
        {
            return Ok(new Response(1, (_db.Deleteable<sysMsgBox>().Where(a => a.id == id).ExecuteCommand() > 0?1:2), "操作成功"));
        }
    }
}
