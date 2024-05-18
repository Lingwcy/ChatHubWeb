using System.Linq.Expressions;

namespace ChatHubApi.Repository.@interface
{
    public interface IRepository<T> where T : class, new()
    {
        Task<T> GetSingle(Expression<Func<T, bool>> whereExpression);
        Task<List<T>> GetList(Expression<Func<T, bool>> whereExpression);
        //Task<List<T>> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page);
        Task<int> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
    }


}
