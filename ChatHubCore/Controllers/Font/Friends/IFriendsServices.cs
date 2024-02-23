

using Microsoft.AspNetCore.Mvc;

namespace construct.Application.System.FontServices.Friends
{
    interface IFriendsServices
    {
        /// <summary>
        /// 添加好友（好友）
        /// 描述:目标表 SysFriends
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public Task<ActionResult<string>> QueryFriendsFind(string targetName, string username);

    }
}
