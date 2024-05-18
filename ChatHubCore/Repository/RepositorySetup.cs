using ChatHubApi.Repository.entity;
using ChatHubApi.Repository.@interface;
using ChatHubApi.System.Entity.Font;
using SqlSugar;

namespace ChatHubApi.Repository
{
    public static class RepositorySetup
    {
        public static void AddRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRepository<sysFontUser>,BaseRepository<sysFontUser>>();
        }
    }
}
