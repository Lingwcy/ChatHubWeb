namespace ChatHubApi.Controllers.Font.Group.Model
{
    public record JoinModel(int GroupId,string UserName,int UserId);
    public record SendGroupRequestModel(int GroupId,int UserId,string ReqMsg);
    public record AcceptGroupRequestModel(int Id);
    public record RejectGroupRequestModel(int Id);
    public record CreateGroupModel(string Name, int[] UserId,int CreateUserId);
    public record ChangeGroupNameModel(string ChangedName,int GroupId,int UserId);
    public record ChangeGroupNoticeModel(string Notice,int GroupId,int UserId);
    public record ExitGroupModel(int GroupId,int UserId);
    public record DismissGroupModel(int GroupId, int UserId);
}
