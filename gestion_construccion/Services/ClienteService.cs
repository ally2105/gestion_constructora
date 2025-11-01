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
    // ClienteService implementa la interfaz IClienteService, cumpliendo el contrato de operaciones para clientes.
    public class ClienteService : IClienteService
    {
        // _unitOfWork es para interactuar con la base de datos de forma transaccional.
        private readonly IUnitOfWork _unitOfWork;
        // _userManager es el servicio de ASP.NET Core Identity para manejar las operaciones de los usuarios (crear, eliminar, etc.).
        private readonly UserManager<Usuario> _userManager;

        // El constructor recibe las dependencias (UnitOfWork y UserManager) a través de inyección de dependencias.
        public ClienteService(IUnitOfWork unitOfWork, UserManager<Usuario> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // Obtiene todos los clientes, incluyendo la información del Usuario asociado para evitar consultas adicionales.
        public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            return await _unitOfWork.Clientes.GetQuery().Include(c => c.Usuario!).ToListAsync();
        }

        // Obtiene un cliente específico por su ID, incluyendo también los datos del Usuario asociado.
        public async Task<Cliente?> GetClienteByIdAsync(int id)
        {
            return await _unitOfWork.Clientes.GetQuery().Include(c => c.Usuario!).FirstOrDefaultAsync(c => c.Id == id);
        }

        // Lógica principal para añadir un nuevo cliente.
        public async Task<Cliente> AddClienteAsync(ClienteViewModel model)
        {
            // 1. Se crea un objeto 'Usuario' de Identity a partir de los datos del ViewModel del formulario.
            var user = new Usuario
            {
                UserName = model.Email, // Es una convención común usar el email como nombre de usuario.
                Email = model.Email,
                Nombre = model.Nombre,
                Identificacion = model.Identificacion,
                // Se especifica que la fecha es de tipo UTC para ser compatible con PostgreSQL.
                FechaNacimiento = DateTime.SpecifyKind(model.FechaNacimiento, DateTimeKind.Utc)
            };

            // 2. Se usa el UserManager para crear el usuario en la base de datos con la contraseña proporcionada.
            // Identity se encarga de hashear la contraseña de forma segura.
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                // Si la creación del usuario falla (ej: email duplicado, contraseña no cumple requisitos), se lanza una excepción con los errores.
                throw new ApplicationException($"Error al crear el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // 3. Una vez creado el usuario, se le asigna el rol de "Cliente".
            await _userManager.AddToRoleAsync(user, "Cliente");

            // 4. Se crea el objeto 'Cliente' de negocio, vinculándolo con el ID del usuario recién creado.
            var cliente = new Cliente(user.Id, model.Direccion);

            // 5. Se añade el nuevo cliente al UnitOfWork y se guardan todos los cambios en la base de datos.
            await _unitOfWork.Clientes.AddAsync(cliente);
            await _unitOfWork.CompleteAsync();
            
            // Se asigna el objeto 'user' a la propiedad de navegación para devolver el objeto completo si es necesario.
            cliente.Usuario = user; 
            return cliente;
        }

        // Lógica para actualizar un cliente existente.
        public async Task<Cliente?> UpdateClienteAsync(int id, ClienteViewModel model)
        {
            // Se obtiene el cliente y su usuario asociado de la base de datos.
            var clienteToUpdate = await GetClienteByIdAsync(id);
            if (clienteToUpdate == null || clienteToUpdate.Usuario == null) 
            {
                return null; // Si no se encuentra, no se puede actualizar.
            }

            // 1. Se actualizan las propiedades del objeto 'Usuario' con los nuevos datos del ViewModel.
            clienteToUpdate.Usuario.Email = model.Email;
            clienteToUpdate.Usuario.UserName = model.Email;
            clienteToUpdate.Usuario.Nombre = model.Nombre;
            clienteToUpdate.Usuario.Identificacion = model.Identificacion;
            clienteToUpdate.Usuario.FechaNacimiento = DateTime.SpecifyKind(model.FechaNacimiento, DateTimeKind.Utc);

            // Se usa el UserManager para persistir los cambios del usuario en la base de datos.
            var result = await _userManager.UpdateAsync(clienteToUpdate.Usuario);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Error al actualizar el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // 2. Se actualizan las propiedades del objeto 'Cliente'.
            clienteToUpdate.Direccion = model.Direccion;
            _unitOfWork.Clientes.Update(clienteToUpdate);
            
            // 3. Se guardan todos los cambios (del usuario y del cliente) en la base de datos.
            await _unitOfWork.CompleteAsync();

            return clienteToUpdate;
        }

        // Lógica para eliminar un cliente.
        public async Task<bool> DeleteClienteAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
            if (cliente == null) return false;

            // 1. Antes de eliminar el cliente, se debe eliminar el usuario de Identity asociado.
            var user = await _userManager.FindByIdAsync(cliente.UsuarioId.ToString());
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    // Si falla la eliminación del usuario, se lanza una excepción para detener el proceso.
                    throw new ApplicationException($"Error al eliminar el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // 2. Una vez eliminado el usuario, se elimina el cliente.
            _unitOfWork.Clientes.Remove(cliente);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // Lógica para buscar clientes.
        public async Task<IEnumerable<Cliente>> SearchClientesAsync(string searchTerm)
        {
            // Si el término de búsqueda está vacío, se devuelven todos los clientes.
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllClientesAsync();
            }

            // Se construye una consulta que busca coincidencias en el Nombre o Identificación del Usuario asociado.
            return await _unitOfWork.Clientes.GetQuery()
                .Include(c => c.Usuario!) // Se incluye el Usuario para poder filtrar por sus propiedades.
                .Where(c => 
                    c.Usuario.Nombre.Contains(searchTerm) || 
                    c.Usuario.Identificacion.Contains(searchTerm))
                .ToListAsync();
        }
    }
}
