using System.Linq.Expressions;

namespace gestion_construccion.Repositories
{
    // Interfaz genérica para el patrón Repositorio.
    // El '<T> where T : class' significa que esta interfaz puede trabajar con cualquier tipo de clase (nuestros modelos).
    public interface IRepository<T> where T : class
    {
        // Obtiene todos los registros de una tabla de forma asíncrona.
        Task<IEnumerable<T>> GetAllAsync();

        // Obtiene un registro por su ID de forma asíncrona.
        Task<T?> GetByIdAsync(int id);

        // Añade un nuevo registro a la tabla de forma asíncrona.
        Task AddAsync(T entity);

        // Marca un registro como modificado. No es asíncrono porque solo cambia el estado en memoria.
        void Update(T entity);

        // Marca un registro como eliminado.
        void Remove(T entity);

        // Busca registros que cumplan con una condición (predicado) de forma asíncrona.
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        // Devuelve un objeto IQueryable, que permite construir consultas más complejas (como Includes) antes de ejecutarlas.
        IQueryable<T> GetQuery();
    }
}
