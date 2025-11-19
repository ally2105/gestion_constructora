using System.Collections.Generic;

namespace Firmeza.Core.Models.ViewModels
{
    public class ImportResultViewModel
    {
        public int FilasProcesadas { get; set; }
        public int VentasCreadas { get; set; }
        public int ClientesNuevos { get; set; }
        public int ProductosNuevos { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
    }
}
