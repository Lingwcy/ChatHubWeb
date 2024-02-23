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
    /// 好友关系表
    /// </summary>
    [SugarTable("t_friends")]
    [Description("好友关系表")]
    public class sysFriends
    {

        /// <summary>
        /// 主键id
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "id", IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "用户id")]
        public int TheUserId { get; set; }

        /// <summary>
        /// 好友id
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "好友id")]
        public int FriendId { get; set; }

        /// <summary>
        /// 好友名称
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "好友名称")]
        public string FriendName { get; set; }

        /// <summary>
        /// 好友头像
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "好友头像")]
        public string FriendImg { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "备注")]
        public string remark { get; set; }
    }
}
