using SqlSugar;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatHubApi.System.Entity.Font
{
    /// <summary>
    /// 用户的树形分组表[前台]
    /// </summary>
    [SugarTable("t_relationtree")]
    [Description("用户树形分组表")]
    public class sysRelationTree
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
        [SugarColumn(ColumnDescription = "所有者ID")]
        public int ownerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "分组名称")]
        public string name { get; set; }
    }
}