using SqlSugar;

namespace ChatHubApi.System
{
    public static class SqlsugarlSetup
    {
        public static void AddSqlsugar(this IServiceCollection services, IConfiguration configuration )
        {
            var configConnection = new ConnectionConfig()
            {
                DbType = DbType.MySql,
                ConnectionString = configuration["ConnectionString"],
                IsAutoCloseConnection = true,
            };
            SqlSugarScope sqlSugar = new SqlSugarScope(configConnection,
                db =>
                {
                    //单例参数配置，所有上下文生效
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        //Console.WriteLine(sql);//输出sql
                    };
                });
            services.AddSingleton<ISqlSugarClient>(sqlSugar);
        }
    }
}
