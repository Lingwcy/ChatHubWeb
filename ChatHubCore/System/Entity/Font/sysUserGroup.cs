using SqlSugar;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ChatHubApi.System.Entity.Font
{
    /// <summary>
    /// 群组用户关系表[前台]
    /// </summary>
    [SugarTable("t_usergroups")]
    [Description("群组用户关系表")]
    public class sysUserGroup
    {

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int UserGroupId { get; set; }

        public int UserId { get; set; }

        public int GroupId { get; set; }

        [SugarColumn(IsNullable = false)]
        public DateTime JoinDate { get; set; }

        public bool IsActive { get; set; }

        [SugarColumn(IsIgnore = true)] 
        public sysFontUser User { get; set; } 
        [SugarColumn(IsIgnore = true)]
        public sysGroups Group { get; set; }
    }
}
