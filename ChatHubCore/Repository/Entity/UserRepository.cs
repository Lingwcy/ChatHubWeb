using ChatHubApi.System.Entity.Font;
using SqlSugar;

namespace ChatHubApi.Repository.entity
{
    public class UserRepository<T> : BaseRepository<sysFontUser>
    {
        public UserRepository(ISqlSugarClient db) : base(db)
        {
            
        }
    }
}
