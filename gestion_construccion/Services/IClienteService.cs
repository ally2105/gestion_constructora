using gestion_construccion.Models;
using gestion_construccion.Models.ViewModels; // <-- AÑADIDO
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gestion_construccion.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllClientesAsync();
        Task<Cliente?> GetClienteByIdAsync(int id);
        Task<Cliente> AddClienteAsync(ClienteViewModel model); // <-- CAMBIADO
        Task<Cliente?> UpdateClienteAsync(int id, ClienteViewModel model); // <-- CAMBIADO
        Task<bool> DeleteClienteAsync(int id);
        Task<IEnumerable<Cliente>> SearchClientesAsync(string searchTerm);
    }
}
