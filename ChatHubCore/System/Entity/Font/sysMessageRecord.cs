using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHubApi.System.Entity.Font
{
    /// <summary>
    /// 消息推送记录表
    /// </summary>
    [SugarTable("t_messagerecord")]
    [Description("消息推送记录表")]
    public class sysMessageRecord
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Required]
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "推送id")]
        public int id { get; set; }


        /// <summary>
        /// 发送者
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "发送者")]
        public string Sender { get; set; }

        /// <summary>
        /// 接受者
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "接受者")]
        public string Receiver { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 消息体
        /// </summary>
        [Required, MaxLength(255)]
        [SugarColumn(ColumnDescription = "消息体")]
        public string SendMessage { get; set; }

        /// <summary>
        /// 发送人头像
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "发送人头像")]
        public string SenderImg { get; set; }
    }
}
