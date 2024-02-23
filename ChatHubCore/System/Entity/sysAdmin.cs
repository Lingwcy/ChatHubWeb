using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHubApi.System.Entity
{
    /// <summary>
    /// 管理员表
    /// </summary>
    [SugarTable("t_admin")]
    [Description("管理员表")]
    public class sysAdmin
    {

        /// <summary>
        /// 用户名
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "用户名")]
        public string name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "密码")]
        public string psw { get; set; }
    }
}
