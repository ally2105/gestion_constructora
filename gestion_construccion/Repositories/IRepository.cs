using System.Linq.Expressions;

namespace gestion_construccion.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        // Nuevo método para obtener un IQueryable que permite Includes
        IQueryable<T> GetQuery();
    }
}
