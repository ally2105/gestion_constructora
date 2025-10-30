using gestion_construccion.Models;
using gestion_construccion.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // <-- AÑADIDO

namespace gestion_construccion.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ILogger<UsuariosController> _logger; // <-- AÑADIDO

        public UsuariosController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, ILogger<UsuariosController> logger) // <-- AÑADIDO
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger; // <-- AÑADIDO
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Comprobar si el usuario tiene el rol 'Cliente'
                    var isClient = await _userManager.IsInRoleAsync(user, "Cliente");
                    if (isClient)
                    {
                        ModelState.AddModelError(string.Empty, "Los clientes no pueden iniciar sesión en esta área.");
                        return View(model);
                    }

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        // --- Lógica de redirección basada en roles ---
                        var isAdmin = await _userManager.IsInRoleAsync(user, "Administrador");
                        _logger.LogInformation("Usuario {Email} inició sesión. ¿Es Administrador? {IsAdmin}", user.Email, isAdmin); // <-- AÑADIDO

                        if (isAdmin)
                        {
                            return RedirectToAction("Dashboard", "Admin"); // Redirigir al admin al dashboard
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home"); // Redirigir a otros usuarios a la página de inicio
                        }
                    }
                }
                ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Usuario { UserName = model.Email, Email = model.Email, Nombre = "Por definir", Identificacion = "Por definir" };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Por defecto, los usuarios registrados desde aquí serán Clientes
                    await _userManager.AddToRoleAsync(user, "Cliente");
                    return RedirectToAction("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
