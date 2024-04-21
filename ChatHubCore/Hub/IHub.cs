namespace ChatHubApi.Hub
{
    public interface IHub
    {
        Task PublicMsgReceived(string headerImage, string fromName, string message);
        Task PrivateMsgReceived(string headerImage, string fromName, string message);
        Task GroupMsgReceived(string headerImage, string fromName, string message, string groupId);
        Task SystemMsgReceived(string message);
        Task FriendsRequestReceived(string fromName);
        Task MsgBoxFlasherReceived(string fromName);
        Task GroupMsgBoxFlasherReceived();
        Task RefreshGroupList();
        //Task HubKeyReceived();
    }
}
