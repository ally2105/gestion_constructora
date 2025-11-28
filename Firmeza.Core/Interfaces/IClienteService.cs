using Firmeza.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces
{
    /// <summary>
    /// Service for client-related operations.
    /// </summary>
    public interface IClienteService
    {
        /// <summary>
        /// Gets all clients.
        /// </summary>
        Task<IEnumerable<Cliente>> GetAllClientesAsync();

        /// <summary>
        /// Gets a client by its identifier.
        /// </summary>
        Task<Cliente?> GetClienteByIdAsync(int id);

        /// <summary>
        /// Creates a client and its associated user account.
        /// </summary>
        /// <param name="cliente">Client entity (without user).</param>
        /// <param name="password">Initial password for the user account.</param>
        Task<Cliente> AddClienteAsync(Cliente cliente, string password);

        /// <summary>
        /// Updates an existing client's data.
        /// </summary>
        Task<Cliente?> UpdateClienteAsync(int id, Cliente cliente);

        /// <summary>
        /// Deletes a client by id.
        /// </summary>
        Task<bool> DeleteClienteAsync(int id);

        /// <summary>
        /// Searches for clients by name, identification, or email.
        /// </summary>
        Task<IEnumerable<Cliente>> SearchClientesAsync(string searchTerm);
    }
}
