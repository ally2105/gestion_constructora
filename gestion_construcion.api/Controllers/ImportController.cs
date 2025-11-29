using Firmeza.Core.Interfaces;
using Firmeza.Core.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Firmeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ImportController : ControllerBase
    {
        private readonly IImportService _importService;

        public ImportController(IImportService importService)
        {
            _importService = importService;
        }

        /// <summary>
        /// Importa datos desde un archivo Excel
        /// </summary>
        /// <param name="file">Archivo Excel (.xlsx o .xls)</param>
        /// <returns>Resultado de la importación con estadísticas y errores</returns>
        [HttpPost("upload")]
        public async Task<ActionResult<ImportResultViewModel>> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Por favor, seleccione un archivo válido." });
            }

            // Validar extensión del archivo
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".xls")
            {
                return BadRequest(new { message = "El archivo debe ser un Excel válido (.xlsx o .xls)." });
            }

            try
            {
                var result = await _importService.ImportarDatosDesdeExcelAsync(file.OpenReadStream());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    message = "Ocurrió un error durante la importación.",
                    error = ex.Message 
                });
            }
        }
    }
}
