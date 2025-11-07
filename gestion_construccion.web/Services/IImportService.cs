using System.IO;
using System.Threading.Tasks;

namespace gestion_construccion.web.Services
{
    public interface IImportService
    {
        Task<ImportResultViewModel> ImportarDatosDesdeExcelAsync(Stream stream);
    }
}
