using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHubApi.System.Entity
{    /// <summary>
     /// 机构表
     /// </summary>
    [SugarTable("t_institution")]
    [Description("机构表")]
    public class sysInstitution
    {

        /// <summary>
        /// 主键
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "序号")]
        public string id { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "单位名称")]
        public string name { get; set; }
    }
}
