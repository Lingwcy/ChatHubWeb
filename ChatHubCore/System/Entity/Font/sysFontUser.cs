using ChatHubApi.System.Enum;
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
    /// 用户表[前台]
    /// </summary>
    [SugarTable("t_fontuser")]
    [Description("前台用户表")]
    public class sysFontUser
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "ID")]
        public int id { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "密码")]
        public string Password { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "用户名")]
        public string Username { get; set; }

        /// <summary>
        ///头像
        /// </summary>
        [Required, MaxLength(255)]
        [SugarColumn(ColumnDescription = "头像路径")]
        public string HeaderImg { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 居住地
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "居住地")]
        public string City { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "性别")]
        public string Sex { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "年龄")]
        public string Age { get; set; }

        /// <summary>
        ///工作
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "工作")]
        public string Job { get; set; }


        /// <summary>
        /// 电话号
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "电话号")]
        public string Phone { get; set; }


        /// <summary>
        /// 社交名称
        /// </summary>
        [Required, MaxLength(30)]
        [SugarColumn(ColumnDescription = "社交名称")]
        public string NickName { get; set; }


        /// <summary>
        /// 生日
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "生日")]
        public string Birth { get; set; }


        /// <summary>
        /// 简介
        /// </summary>
        [Required, MaxLength(255)]
        [SugarColumn(ColumnDescription = "简介")]
        public string Desc { get; set; }


        /// <summary>
        /// 状态-正常_0、停用_1、删除_2
        /// </summary>
        [SugarColumn(ColumnDescription = "状态-正常_0、停用_1、删除_2")]
        public Status status { get; set; } = Status.ENABLE;
    }
}
