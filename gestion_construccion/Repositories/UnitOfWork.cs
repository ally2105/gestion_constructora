
using gestion_construccion.Datos;

namespace gestion_construccion.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // Repositorios específicos
        // public IPersonaRepository Personas { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            // Inicializar repositorios
            // Personas = new PersonaRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
