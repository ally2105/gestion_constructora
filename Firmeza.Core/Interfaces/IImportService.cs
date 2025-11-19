using System.IO;
using System.Threading.Tasks;
using Firmeza.Core.Models.ViewModels; // Nuevo using para ImportResultViewModel

namespace Firmeza.Core.Interfaces
{
    public interface IImportService
    {
        Task<ImportResultViewModel> ImportarDatosDesdeExcelAsync(Stream stream);
    }
}
