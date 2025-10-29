
namespace gestion_construccion.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositorios específicos
        // Ejemplo: IPersonaRepository Personas { get; }

        Task<int> CompleteAsync();
    }
}
