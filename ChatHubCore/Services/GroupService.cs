using ChatHubApi.Hub;
using ChatHubApi.System.Entity.Font;
using ChatHubApi.System;
using SqlSugar;

namespace ChatHubApi.Services
{
    public class GroupService: IGroupService
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogger<MyHub> _logger;

        public GroupService(ISqlSugarClient db, ILogger<MyHub> logger)
        {
            _db = db;
            _logger = logger;
        }

        //返回群组中的成员
        public async Task<List<sysFontUser>> GetGroupMembers(int groupId)
        {
            var flag = _db.Queryable<sysGroups>().Where(e => e.GroupId == groupId).Count();
            if (flag == 0)
            { return null; }
            /***
             * 因为UserGroup的类型是List<sysUserGroup>
             * 此时Mapper方法根据GroupId查询UserGroup，然后将UserGroup的类型与实体sysUserGroup进行映射
             */
            var userList = _db.Queryable<sysGroups>()
                .Mapper(it => it.UserGroup, it => it.GroupId)
                .First(x => x.GroupId == groupId)
                .UserGroup
                .Select(x => (x.UserId, x.Role));
            List<sysFontUser> memberList = new List<sysFontUser>();
            foreach (var user in userList)
            {
                var res = await _db.Queryable<sysFontUser>()
                     .FirstAsync(x => x.id == user.UserId);
                memberList.Add(res);
            }
            return memberList;
        }

        //返回群组中的在线成员的conid
        public async Task<List<string>> GetOnlineGroupMembers(int groupId)
        {  
            List<sysFontUser> memberList = await GetGroupMembers(groupId);
            if (memberList == null)
            {
                return new List<string>();
            }
            List<string> onlineList = new List<string>();
            foreach (var member in memberList)
            {
                var userId = GetOnlineId(member.id);
                if (userId != null)
                {
                    onlineList.Add(userId);
                }
            }
            return onlineList;
        }

        //查询成员在线conid
        public string GetOnlineId(int userId)
        {
            var user =  _db.Queryable<sysOnlineUser>()
               .First(x => int.Parse(x.userid) == userId);
            if (user != null)
            {
                return user.conId;
            }
            return null;
        }
    }
}
