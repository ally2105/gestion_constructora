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
        // Se devuelve IQueryable para permitir la construcción de consultas complejas.
        IQueryable<T> GetQuery();
    }
}
