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
    }
}
