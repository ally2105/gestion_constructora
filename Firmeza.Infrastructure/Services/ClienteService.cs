using Firmeza.Core.Models;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Firmeza.Infrastructure.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(IUnitOfWork unitOfWork, UserManager<Usuario> userManager, IEmailService emailService, ILogger<ClienteService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
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
            _logger.LogInformation("Iniciando AddClienteAsync para el email {Email}", cliente.Usuario.Email);
            var user = cliente.Usuario;

            if (string.IsNullOrEmpty(user.UserName))
            {
                user.UserName = user.Email;
            }

            user.FechaNacimiento = DateTime.SpecifyKind(user.FechaNacimiento, DateTimeKind.Utc);
            _logger.LogInformation("Intentando crear usuario en UserManager...");

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                _logger.LogError("Error en UserManager.CreateAsync: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new ApplicationException($"Error al crear el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            _logger.LogInformation("Usuario creado con éxito con ID {UserId}", user.Id);

            _logger.LogInformation("Añadiendo usuario al rol 'Cliente'...");
            await _userManager.AddToRoleAsync(user, "Cliente");
            _logger.LogInformation("Usuario añadido al rol 'Cliente' con éxito.");

            cliente.UsuarioId = user.Id;
            cliente.Usuario = null!;

            _logger.LogInformation("Añadiendo cliente al UnitOfWork...");
            await _unitOfWork.Clientes.AddAsync(cliente);
            _logger.LogInformation("Guardando cambios en UnitOfWork...");
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Cliente guardado con éxito en la base de datos.");
            
            // Enviar Correo de Bienvenida
            var subject = "¡Bienvenido a Firmeza Construcción!";
            var message = $"<h1>Hola {user.Nombre},</h1><p>Tu cuenta ha sido creada exitosamente. ¡Gracias por unirte a nosotros!</p>";
            await _emailService.SendEmailAsync(user.Email!, subject, message);

            cliente.Usuario = user;
            return cliente;
        }

        // ... (resto de los métodos)
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

        public async Task<Cliente?> GetClienteByUsuarioIdAsync(int usuarioId)
        {
            return await _unitOfWork.Clientes.GetQuery()
                .Include(c => c.Usuario!)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
        }
    }
}
