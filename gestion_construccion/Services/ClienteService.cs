using gestion_construccion.Models;
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
            return await _unitOfWork.Clientes.GetAllAsync();
        }

        public async Task<Cliente?> GetClienteByIdAsync(int id)
        {
            return await _unitOfWork.Clientes.GetByIdAsync(id);
        }

        public async Task<Cliente> AddClienteAsync(Cliente cliente, string password)
        {
            // Crear el usuario de Identity
            var user = new Usuario
            {
                UserName = cliente.Usuario.Email, // Usar el email como UserName
                Email = cliente.Usuario.Email,
                Nombre = cliente.Usuario.Nombre,
                Identificacion = cliente.Usuario.Identificacion,
                FechaNacimiento = cliente.Usuario.FechaNacimiento
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                // Manejar el error de creación de usuario (ej. lanzar excepción o devolver null)
                throw new ApplicationException($"Error al crear el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Asignar el rol 'Cliente'
            await _userManager.AddToRoleAsync(user, "Cliente");

            // Vincular el cliente al usuario creado
            cliente.UsuarioId = user.Id;
            cliente.Usuario = user; // Asegurar la referencia para EF

            await _unitOfWork.Clientes.AddAsync(cliente);
            await _unitOfWork.CompleteAsync();
            return cliente;
        }

        public async Task<Cliente?> UpdateClienteAsync(Cliente cliente)
        {
            // Actualizar el usuario de Identity asociado
            var user = await _userManager.FindByIdAsync(cliente.UsuarioId.ToString());
            if (user == null) return null; // O lanzar excepción

            user.Email = cliente.Usuario.Email;
            user.UserName = cliente.Usuario.Email;
            user.Nombre = cliente.Usuario.Nombre;
            user.Identificacion = cliente.Usuario.Identificacion;
            user.FechaNacimiento = cliente.Usuario.FechaNacimiento;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Error al actualizar el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            _unitOfWork.Clientes.Update(cliente);
            await _unitOfWork.CompleteAsync();
            return cliente;
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

            // Incluir el Usuario para poder buscar por sus propiedades
            return await _unitOfWork.Clientes.FindAsync(c => 
                c.Usuario.Nombre.Contains(searchTerm) || 
                c.Usuario.Identificacion.Contains(searchTerm));
        }
    }
}
