using gestion_construccion.Models;
using gestion_construccion.Models.ViewModels;
using gestion_construccion.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gestion_construccion.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Usuario> _userManager;

        public ClienteService(IUnitOfWork unitOfWork, UserManager<Usuario> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            return await _unitOfWork.Clientes.GetQuery().Include(c => c.Usuario!).ToListAsync();
        }

        public async Task<Cliente?> GetClienteByIdAsync(int id)
        {
            return await _unitOfWork.Clientes.GetQuery().Include(c => c.Usuario!).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente> AddClienteAsync(ClienteViewModel model)
        {
            // 1. Crear el usuario de Identity a partir del ViewModel
            var user = new Usuario
            {
                UserName = model.Email,
                Email = model.Email,
                Nombre = model.Nombre,
                Identificacion = model.Identificacion,
                // Asegurarse de que la fecha se guarda como UTC
                FechaNacimiento = DateTime.SpecifyKind(model.FechaNacimiento, DateTimeKind.Utc)
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Error al crear el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // 2. Asignar el rol 'Cliente'
            await _userManager.AddToRoleAsync(user, "Cliente");

            // 3. Crear el Cliente y vincularlo al usuario recién creado
            var cliente = new Cliente(user.Id, model.Direccion);

            await _unitOfWork.Clientes.AddAsync(cliente);
            await _unitOfWork.CompleteAsync();
            
            cliente.Usuario = user; // Asignar para devolver el objeto completo
            return cliente;
        }

        public async Task<Cliente?> UpdateClienteAsync(int id, ClienteViewModel model)
        {
            var clienteToUpdate = await GetClienteByIdAsync(id);
            if (clienteToUpdate == null || clienteToUpdate.Usuario == null) 
            {
                return null;
            }

            // Actualizar el usuario de Identity asociado
            clienteToUpdate.Usuario.Email = model.Email;
            clienteToUpdate.Usuario.UserName = model.Email;
            clienteToUpdate.Usuario.Nombre = model.Nombre;
            clienteToUpdate.Usuario.Identificacion = model.Identificacion;
            // Asegurarse de que la fecha se guarda como UTC
            clienteToUpdate.Usuario.FechaNacimiento = DateTime.SpecifyKind(model.FechaNacimiento, DateTimeKind.Utc);

            var result = await _userManager.UpdateAsync(clienteToUpdate.Usuario);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Error al actualizar el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Actualizar el cliente
            clienteToUpdate.Direccion = model.Direccion;
            _unitOfWork.Clientes.Update(clienteToUpdate);
            await _unitOfWork.CompleteAsync();

            return clienteToUpdate;
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
            if (cliente == null) return false;

            // Eliminar el usuario de Identity asociado
            var user = await _userManager.FindByIdAsync(cliente.UsuarioId.ToString());
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Error al eliminar el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            _unitOfWork.Clientes.Remove(cliente);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<Cliente>> SearchClientesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllClientesAsync();
            }

            return await _unitOfWork.Clientes.GetQuery()
                .Include(c => c.Usuario!)
                .Where(c => 
                    c.Usuario.Nombre.Contains(searchTerm) || 
                    c.Usuario.Identificacion.Contains(searchTerm))
                .ToListAsync();
        }
    }
}
