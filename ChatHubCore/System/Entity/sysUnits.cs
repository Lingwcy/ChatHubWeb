using SqlSugar;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatHubApi.System.Entity
{
    /// <summary>
    /// 部门表
    /// </summary>
    [SugarTable("t_units")]
    [Description("部门表")]

    public class sysUnits
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
        public string unit { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "部门名称/职位")]
        public string department { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime createtime { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "机构名称")]
        public string institution { get; set; }
    }
}
