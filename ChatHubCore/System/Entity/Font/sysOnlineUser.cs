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
    /// 在线人员记录表(缓存表)
    /// </summary>
    [SugarTable("t_onlieuser")]
    [Description("在线人员记录表")]
    public class sysOnlineUser
    {

        /// <summary>
        /// 主键
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "id")]
        public int id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "用户名")]
        public string name { get; set; }


        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "用户id")]
        public string userid { get; set; }

        /// <summary>
        /// 加入时间
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "加入时间")]
        public DateTime createtime { get; set; }


        /// <summary>
        /// 连接id
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "连接id")]
        public string conId { get; set; }

        /// <summary>
        /// Hub KEY
        /// </summary>
        [Required, MaxLength(255)]
        [SugarColumn(ColumnDescription = "中心key")]
        public string key { get; set; }
    }
}
