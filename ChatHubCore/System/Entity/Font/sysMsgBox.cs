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
    /// 消息盒子表
    /// </summary>
    [SugarTable("t_msgbox")]
    [Description("消息盒子表")]
    public class sysMsgBox
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "主键消息id", IsPrimaryKey = true)]
        public int id { get; set; }

        /// <summary>
        /// 记录用户
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "记录用户")]
        public string username { get; set; }

        /// <summary>
        /// 展示内容(一般为目标用户名称)
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "展示内容")]
        public string targetfont { get; set; }

        /// <summary>
        ///目标图片
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "目标图片")]
        public string targetImage { get; set; }


        /// <summary>
        ///是否为未读消息
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "是否未读")]
        public int isNew { get; set; }

        /// <summary>
        ///消息盒子类型
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "消息盒子类型")]
        public string Type { get; set; }



    }
}
