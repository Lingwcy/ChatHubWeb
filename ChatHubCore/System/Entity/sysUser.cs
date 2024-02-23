using ChatHubApi.System.Enum;
using SqlSugar;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatHubApi.System.Entity
{
    /// <summary>
    /// 用户表
    /// </summary>
    [SugarTable("t_user")]
    [Description("用户表")]
    public class sysUser
    {


        /// <summary>
        /// 密码
        /// </summary>
        [Required, MaxLength(50)]
        [SugarColumn(ColumnDescription = "密码")]
        public string psw { get; set; }

        /// <summary>
        /// 网络昵称
        /// </summary>
        [MaxLength(20)]
        [SugarColumn(ColumnDescription = "昵称", IsNullable = true)]
        public string name { get; set; }

        /// <summary>
        /// 性别-男_1、女_2
        /// </summary>
        [SugarColumn(ColumnDescription = "性别-男_1、女_2")]
        public Gender sex { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(20)]
        [SugarColumn(ColumnDescription = "姓名", IsNullable = true)]
        public string trueName { get; set; }


        /// <summary>
        /// 头像
        /// </summary>
        [SugarColumn(ColumnDescription = "头像", IsNullable = true)]
        public string avatar { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(20)]
        [SugarColumn(ColumnDescription = "手机", IsNullable = true)]
        public string phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(20)]
        [SugarColumn(ColumnDescription = "邮箱", IsNullable = true)]
        public string email { get; set; }

        /// <summary>
        /// 状态-正常_0、停用_1、删除_2
        /// </summary>
        [SugarColumn(ColumnDescription = "状态-正常_0、停用_1、删除_2")]
        public Status status { get; set; } = Status.ENABLE;

        /// <summary>
        /// 状态-在线_0、离线_1、异常_2
        /// </summary>
        [SugarColumn(ColumnDescription = "状态-在线_0、离线_1、异常_2")]
        public OnlineStatus online { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(20)]
        [SugarColumn(ColumnDescription = "备注", IsNullable = true)]
        public string mark { get; set; }



        /// <summary>
        /// 角色
        /// </summary>
        [MaxLength(20)]
        [SugarColumn(ColumnDescription = "角色", IsNullable = false)]
        public string role { get; set; }

        /// <summary>
        /// qq号
        /// </summary>
        [MaxLength(20)]
        [SugarColumn(ColumnDescription = "qq", IsNullable = true)]
        public string qq { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [MaxLength(30)]
        [SugarColumn(ColumnDescription = "单位", IsNullable = true)]
        public string institution { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [MaxLength(30)]
        [SugarColumn(ColumnDescription = "部门", IsNullable = true)]
        public string department { get; set; }


        public string IMEL { get; set; }

    }
}
