
using System.ComponentModel;


namespace ChatHubApi.System.Enum
{
    public enum Status
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        ENABLE = 0,

        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        DISABLE = 1,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        DELETED = 2
    }
}
