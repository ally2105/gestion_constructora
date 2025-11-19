using Firmeza.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllClientesAsync();
        Task<Cliente?> GetClienteByIdAsync(int id);
        Task<Cliente> AddClienteAsync(Cliente cliente, string password); // ¡Añadido el parámetro password!
        Task<Cliente?> UpdateClienteAsync(int id, Cliente cliente);
        Task<bool> DeleteClienteAsync(int id);
        Task<IEnumerable<Cliente>> SearchClientesAsync(string searchTerm);
    }
}
