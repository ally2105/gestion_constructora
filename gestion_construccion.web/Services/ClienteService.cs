using gestion_construccion.web.Models;
using gestion_construccion.web.Models.ViewModels;
using gestion_construccion.web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gestion_construccion.web.Services
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

        public async Task<Cliente> AddClienteAsync(ClienteViewModel model)
        {
            var user = new Usuario
            {
                UserName = model.Email,
                Email = model.Email,
                Nombre = model.Nombre,
                Identificacion = model.Identificacion,
                FechaNacimiento = DateTime.SpecifyKind(model.FechaNacimiento, DateTimeKind.Utc),
                PhoneNumber = model.Telefono
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Error al crear el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await _userManager.AddToRoleAsync(user, "Cliente");

            var cliente = new Cliente(user.Id, model.Direccion);

            await _unitOfWork.Clientes.AddAsync(cliente);
            await _unitOfWork.CompleteAsync();
            
            // --- Enviar Correo de Bienvenida ---
            var subject = "¡Bienvenido a Firmeza Construcción!";
            var message = $"<h1>Hola {user.Nombre},</h1><p>Tu cuenta ha sido creada exitosamente. ¡Gracias por unirte a nosotros!</p>";
            await _emailService.SendEmailAsync(user.Email, subject, message);

            cliente.Usuario = user;
            return cliente;
        }

        public async Task<Cliente?> UpdateClienteAsync(int id, ClienteEditViewModel model) 
        {
            var clienteToUpdate = await GetClienteByIdAsync(id);
            if (clienteToUpdate == null || clienteToUpdate.Usuario == null) 
            {
                return null;
            }

            clienteToUpdate.Usuario.Email = model.Email;
            clienteToUpdate.Usuario.UserName = model.Email;
            clienteToUpdate.Usuario.Nombre = model.Nombre;
            clienteToUpdate.Usuario.Identificacion = model.Identificacion;
            clienteToUpdate.Usuario.FechaNacimiento = DateTime.SpecifyKind(model.FechaNacimiento, DateTimeKind.Utc);
            clienteToUpdate.Usuario.PhoneNumber = model.Telefono;

            var result = await _userManager.UpdateAsync(clienteToUpdate.Usuario);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Error al actualizar el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            clienteToUpdate.Direccion = model.Direccion;
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
