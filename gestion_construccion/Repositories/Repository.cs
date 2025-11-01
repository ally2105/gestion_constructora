using gestion_construccion.Datos;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace gestion_construccion.Repositories
{
    // Implementación genérica de la interfaz IRepository.
    public class Repository<T> : IRepository<T> where T : class
    {
        // Referencia al contexto de la base de datos.
        private readonly AppDbContext _context;
        // Referencia al DbSet específico para el tipo T (ej: DbSet<Producto>).
        private readonly DbSet<T> _dbSet;

        // El constructor recibe el contexto y obtiene el DbSet correspondiente al tipo T.
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Implementación para obtener todos los registros.
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Implementación para obtener un registro por ID.
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Implementación para añadir un nuevo registro.
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // Implementación para marcar un registro como modificado.
        public void Update(T entity)
        {
            _dbSet.Attach(entity); // Se adjunta la entidad al contexto.
            _context.Entry(entity).State = EntityState.Modified; // Se cambia su estado a "Modificado".
        }

        // Implementación para marcar un registro como eliminado.
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        // Implementación para buscar registros que cumplan una condición.
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        // Implementación para devolver el DbSet como un IQueryable.
        public IQueryable<T> GetQuery()
        {
            return _dbSet;
        }
    }
}
