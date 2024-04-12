using SqlSugar;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatHubApi.System.Entity.Font
{
    /// <summary>
    /// 用户的树形分组成员[前台]
    /// </summary>
    [SugarTable("t_relationtreemember")]
    [Description("用户的树形分组成员")]
    public class systRelationTreeMember
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "ID")]
        public int id { get; set; }
        /// <summary>
        /// 所有者ID
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "分组ID")]
        public int groupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "分组中的用户ID")]
        public int nameId { get; set; }
    }
}
