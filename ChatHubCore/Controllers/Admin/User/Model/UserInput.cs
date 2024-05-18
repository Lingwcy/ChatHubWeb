
using ChatHubApi.System.Enum;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ChatHubApi.Controllers.AdminServices.User.Model
{
    public class UserInput
    {
        /// <summary>
        /// 密码
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// 网络昵称
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// 性别-男_1、女_2
        /// </summary>
        public virtual string? Sex { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public virtual string? HeaderImg { get; set; }


        /// <summary>
        /// 头像
        /// </summary>
        public virtual string? Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public virtual string? City { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public virtual string? Age { get; set; }

        /// <summary>
        /// 状态-正常_0、停用_1、删除_2
        /// </summary>
        public virtual string? Job { get; set; }

        /// <summary>
        /// 状态-在线_0、离线_1、异常_2
        /// </summary>
        public virtual int? Status { get; set; } = 1;

        public virtual int? id { get; set; }

        public virtual string? Phone { get; set; }

        public virtual string? NickName { get; set; }

        public virtual string? Birth { get; set; }

        public virtual string? Desc { get; set; }

    }


    public record SearchModel(string? username,string? phone);
    public record OnlineUserSearchModel(string? name);
    public record DeleteUserInput(int id);

    public class AddUserInput : UserInput
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        public override string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        public override string Password { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Required(ErrorMessage = "头像不能为空")]
        public override string HeaderImg { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public override string? NickName { get; set; }

        public override string? Phone { get; set; }

        public override string? Email { get; set; }

        public override int? Status { get;set; }

    }


    public class EditUserInput : UserInput
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        public override string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public override string? Password { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Required(ErrorMessage = "头像不能为空")]
        public override string HeaderImg { get; set; }
    }


    public class UserInfo
    {
        public int id { get; set; }
        public string Username { get; set; }
        public string HeaderImg { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Sex { get; set; }
        public string Age { get; set; }
        public string Job { get; set; }
        public string Phone { get; set; }
        public string NickName { get; set; }
        public string Birth { get; set; }
        public string Desc { get; set; }
        public Status status { get; set; } = Status.ENABLE;
    }


}