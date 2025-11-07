using System.Collections.Generic;
using System.Threading.Tasks;
using gestion_construccion.web.Models;
using gestion_construccion.web.Models.ViewModels;

namespace gestion_construccion.web.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllClientesAsync();
        Task<Cliente?> GetClienteByIdAsync(int id);
        Task<Cliente> AddClienteAsync(ClienteViewModel model);
        Task<Cliente?> UpdateClienteAsync(int id, ClienteEditViewModel model); // <-- CAMBIADO
        Task<bool> DeleteClienteAsync(int id);
        Task<IEnumerable<Cliente>> SearchClientesAsync(string searchTerm);
    }
}
