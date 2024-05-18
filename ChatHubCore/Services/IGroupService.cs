using ChatHubApi.System.Entity.Font;

namespace ChatHubApi.Services
{
    public interface IGroupService
    {
        Task<List<sysFontUser>> GetGroupMembers(int groupId);
        Task<List<string>> GetOnlineGroupMembers(int groupId);
        string GetOnlineId(int userId);
    }
}
