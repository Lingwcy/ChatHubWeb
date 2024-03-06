using SqlSugar;
using System.ComponentModel;

namespace ChatHubApi.System.Entity.Font
{
    /// <summary>
    /// 群组[前台]
    /// </summary>
    [SugarTable("t_group")]
    [Description("群组表")]
    public class sysGroups
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int GroupId { get; set; }

        [SugarColumn(IsNullable = false)]
        public string GroupName { get; set; }

        public string GroupDescription { get; set; }

        [SugarColumn(IsNullable = false)]
        public DateTime CreationDate { get; set; }

        public int? CreatorUserId { get; set; }

        public bool IsDeleted { get; set; }
 
        [SugarColumn(IsIgnore = true)]
        public sysFontUser CreatorUser { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<sysUserGroup> UserGroup { get; set; }
    }
}
