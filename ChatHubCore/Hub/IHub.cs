namespace ChatHubApi.Hub
{
    public interface IHub
    {
        Task PublicMsgReceived(string headerImage, string fromName, string message);
        Task PrivateMsgReceived(string headerImage, string fromName, string message);
        Task GroupMsgReceived(string headerImage, string fromName, string message, string groupId);
        Task SystemMsgReceived(string message);
        Task FriendsRequestReceived(string fromName);
        Task MsgBoxFlasherReceived();
        Task GroupMsgBoxFlasherReceived();
        Task RefreshGroupList();
        Task PublicImageReceived(string headerImage, string fromName, object imageUrl);
        //Task HubKeyReceived();

        //拒绝好友请求通知
        Task FriendRequestRefused(string fromName);
        //同意好友请求通知
        Task FriendRequestAccepted(string fromName);
        //刷新好友列表
        Task RefreshFriendList();
        //刷新群组公告
        Task RefreshGroupNotice(string groupName, string notice);
        //退出群组任务 type 0:被解散 1:主动退出 2:被踢出
        Task DissolveGroupNotice(int groupId, ExiteGroupType type);
        //刷新群名称
        Task RefreshGroupName(int groupId, string groupName);

       
    }

    public enum ExiteGroupType
    {
        Dissolve = 0,
        Exit = 1,
        Kick = 2
    }
}
