using System.Linq.Expressions;

namespace Firmeza.Core.Interfaces
{
    /// <summary>
    /// Generic repository interface for CRUD operations on entities.
    /// </summary>
    /// <typeparam name="T">Entity type (must be a class).</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets all entities of type T.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets an entity by its identifier.
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new entity (changes pending persistence).
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Marks an entity as modified.
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Marks an entity for deletion.
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Finds entities matching a LINQ predicate.
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Returns an IQueryable for building complex queries.
        /// </summary>
        IQueryable<T> GetQuery();
    }
}
