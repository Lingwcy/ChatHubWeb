using SqlSugar;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatHubApi.System.Entity.Font
{
    /// <summary>
    /// 群组请求表
    /// </summary>
    [SugarTable("t_grouprequest")]
    [Description("群组请求表")]
    public class sysGroupReuqest
    {    /// <summary>
         /// id
         /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "id")]
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "用户id")]
        public int UserId { get; set; }


        /// <summary>
        /// 群ID
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "群组ID")]
        public int TargetGroupId { get; set; }

        /// <summary>
        /// 验证信息
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "验证信息")]
        public string ReqMsg { get; set; }

    }
}
