using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace ChatHubApi.Controllers.Font.Friends.Model
{
    public class acceptRequestModel
    {
        public string UserImg { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TargetId { get; set; }
        public string TargetName { get; set; }
        public string TargetImg { get; set; }
        public string remark { get; set; }
        public string ReqMsg { get; set; }

        /// <summary>
        /// 将要添加进好友请求发送者的分组ID
        /// </summary>
        public int TargetGroupId { get; set; }
        /// <summary>
        /// 将要添加进好友请求的接收者的分组ID
        /// </summary>
        public int AccepterGroupId { get; set; }

    }

    public class DenyRequestModel
    {
        public string UserImg { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TargetId { get; set; }
        public string TargetName { get; set; }
        public string TargetImg { get; set; }


    }
}
