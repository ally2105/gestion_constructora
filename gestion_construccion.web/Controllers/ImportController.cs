using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Firmeza.Core.Interfaces; // Nuevo using para IImportService
using Firmeza.Core.Models.ViewModels; // Nuevo using para ImportResultViewModel

namespace gestion_construccion.web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ImportController : Controller
    {
        private readonly IImportService _importService;

        public ImportController(IImportService importService)
        {
            _importService = importService;
        }

        // Muestra la página para subir el archivo.
        public IActionResult Index()
        {
            return View();
        }

        // Procesa el archivo subido.
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Por favor, seleccione un archivo.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var result = await _importService.ImportarDatosDesdeExcelAsync(file.OpenReadStream());
                // Guardar el resultado en TempData para mostrarlo en la vista de resultados.
                TempData["ImportResult"] = System.Text.Json.JsonSerializer.Serialize(result);
                return RedirectToAction(nameof(Result));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error inesperado durante la importación: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Muestra el resultado de la importación.
        public IActionResult Result()
        {
            var resultJson = TempData["ImportResult"] as string;
            if (string.IsNullOrEmpty(resultJson))
            {
                return RedirectToAction(nameof(Index));
            }

            var result = System.Text.Json.JsonSerializer.Deserialize<ImportResultViewModel>(resultJson);
            return View(result);
        }
    }
}
