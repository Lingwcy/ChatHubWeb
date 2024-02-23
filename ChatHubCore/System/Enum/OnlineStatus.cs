
using System.ComponentModel;


namespace ChatHubApi.System.Enum
{
    public enum OnlineStatus
    {
        /// <summary>
        /// 在线
        /// </summary>
        [Description("在线")]
        ON = 0,

        /// <summary>
        /// 离线
        /// </summary>
        [Description("离线")]
        OFF = 1,

        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        ERROR = 2
    }
}