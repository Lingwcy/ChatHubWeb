using ChatHubApi.System.Entity.Font;

namespace ChatHubApi.Controllers.Font.Friends.Model
{
    public class FriendTree
    {
        public long? OnwerId { get; set; }
        public List<FriendTreeUnit> Units { get; set; } = new List<FriendTreeUnit>();

    }

    public class FriendTreeUnit {
        public long id { get; set; }
        public string? UnitName { get; set; }
        public List<sysFontUser>? Children { get; set; } = new List<sysFontUser>();

    }
    
}
