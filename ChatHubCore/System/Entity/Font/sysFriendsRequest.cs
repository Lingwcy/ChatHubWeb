using SqlSugar;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatHubApi.System.Entity.Font
{
    /// <summary>
    /// 好友请求表
    /// </summary>
    [SugarTable("t_friendsrequest")]
    [Description("好友请求表")]
    public class sysFriendsRequest
    {

        /// <summary>
        /// 用户id
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "用户头像")]
        public string UserImg { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "用户id")]
        public int UserId { get; set; }


        /// <summary>
        /// 用户名
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 请求目标用户id
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "目标用户id")]
        public int TargetId { get; set; }

        /// <summary>
        /// 请求目标用户名
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "目标用户名")]
        public string TargetName { get; set; }


        /// <summary>
        /// 请求目标用户头像
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "目标用户头像")]
        public string TargetImg { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(50)]
        [SugarColumn(ColumnDescription = "备注")]
        public string remark { get; set; }

        /// <summary>
        /// 请求消息
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "请求消息")]
        public string ReqMsg { get; set; }
    }
}