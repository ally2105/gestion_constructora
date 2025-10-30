using gestion_construccion.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gestion_construccion.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllClientesAsync();
        Task<Cliente?> GetClienteByIdAsync(int id);
        Task<Cliente> AddClienteAsync(Cliente cliente, string password);
        Task<Cliente?> UpdateClienteAsync(Cliente cliente);
        Task<bool> DeleteClienteAsync(int id);
        Task<IEnumerable<Cliente>> SearchClientesAsync(string searchTerm);
    }
}
