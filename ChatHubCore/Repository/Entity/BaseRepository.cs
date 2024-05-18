using ChatHubApi.Repository.@interface;
using SqlSugar;
using System.Linq.Expressions;

namespace ChatHubApi.Repository.entity
{
    public class BaseRepository<T> : IRepository<T> where T : class, new()
    {
        private readonly ISqlSugarClient _db;

        public BaseRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public async Task<T> GetSingle(Expression<Func<T, bool>> whereExpression)
        {
            return await Task.Run(() => _db.Queryable<T>().First(whereExpression));
        }

        public async Task<List<T>> GetList(Expression<Func<T, bool>> whereExpression)
        {
            return await Task.Run(() => _db.Queryable<T>().Where(whereExpression).ToList());
        }

        public async Task<int> Add(T entity)
        {
            return await _db.Insertable(entity).ExecuteCommandAsync();
        }

        public async Task<bool> Update(T entity)
        {
            return await _db.Updateable(entity).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(T entity)
        {
            return await _db.Deleteable(entity).ExecuteCommandAsync() > 0;
        }
    }



}
