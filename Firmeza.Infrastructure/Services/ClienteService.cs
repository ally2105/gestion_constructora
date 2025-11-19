using Firmeza.Core.Models;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Necesario para DateTime

namespace Firmeza.Infrastructure.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailService _emailService; 

        public ClienteService(IUnitOfWork unitOfWork, UserManager<Usuario> userManager, IEmailService emailService) 
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService; 
        }

        public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            return await _unitOfWork.Clientes.GetQuery().Include(c => c.Usuario!).ToListAsync();
        }

        public async Task<Cliente?> GetClienteByIdAsync(int id)
        {
            return await _unitOfWork.Clientes.GetQuery().Include(c => c.Usuario!).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente> AddClienteAsync(Cliente cliente, string password)
        {
            var user = cliente.Usuario;

            if (string.IsNullOrEmpty(user.UserName))
            {
                user.UserName = user.Email;
            }

            // Asegurarse de que FechaNacimiento sea UTC antes de crear el usuario
            user.FechaNacimiento = DateTime.SpecifyKind(user.FechaNacimiento, DateTimeKind.Utc);

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Error al crear el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await _userManager.AddToRoleAsync(user, "Cliente");

            cliente.UsuarioId = user.Id;

            await _unitOfWork.Clientes.AddAsync(cliente);
            await _unitOfWork.CompleteAsync();
            
            // --- Enviar Correo de Bienvenida (DESHABILITADO TEMPORALMENTE) ---
            // var subject = "¡Bienvenido a Firmeza Construcción!";
            // var message = $"<h1>Hola {user.Nombre},</h1><p>Tu cuenta ha sido creada exitosamente. ¡Gracias por unirte a nosotros!</p>";
            // await _emailService.SendEmailAsync(user.Email!, subject, message);

            return cliente;
        }

        public async Task<Cliente?> UpdateClienteAsync(int id, Cliente cliente) 
        {
            var clienteToUpdate = await GetClienteByIdAsync(id);
            if (clienteToUpdate == null || clienteToUpdate.Usuario == null) 
            {
                return null;
            }

            clienteToUpdate.Usuario.Email = cliente.Usuario.Email;
            clienteToUpdate.Usuario.UserName = cliente.Usuario.Email;
            clienteToUpdate.Usuario.Nombre = cliente.Usuario.Nombre;
            clienteToUpdate.Usuario.Identificacion = cliente.Usuario.Identificacion;
            // Asegurarse de que FechaNacimiento sea UTC antes de actualizar el usuario
            clienteToUpdate.Usuario.FechaNacimiento = DateTime.SpecifyKind(cliente.Usuario.FechaNacimiento, DateTimeKind.Utc);
            clienteToUpdate.Usuario.PhoneNumber = cliente.Usuario.PhoneNumber;

            var result = await _userManager.UpdateAsync(clienteToUpdate.Usuario);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Error al actualizar el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            clienteToUpdate.Direccion = cliente.Direccion;
            _unitOfWork.Clientes.Update(clienteToUpdate);
            await _unitOfWork.CompleteAsync();

            return clienteToUpdate;
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
            if (cliente == null) return false;

            var user = await _unitOfWork.Usuarios.GetQuery().FirstOrDefaultAsync(u => u.Id == cliente.UsuarioId);
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
