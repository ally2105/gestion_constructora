using Firmeza.Core.Models;
using Firmeza.Core.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Threading.Tasks;

namespace Firmeza.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        public async Task<string> GenerarReciboVentaAsync(Venta venta, string basePath)
        {
            var uploadsDir = Path.Combine(basePath, "recibos");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }
            var fileName = $"Recibo-Venta-{venta.Id}-{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(uploadsDir, fileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter.GetInstance(document, fileStream);
                document.Open();

                var fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var fontBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                document.Add(new Paragraph("Recibo de Venta", fontTitle) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 });

                document.Add(new Paragraph($"Fecha: {venta.Fecha:dd/MM/yyyy}", fontNormal));
                document.Add(new Paragraph($"Número de Venta: {venta.Id}", fontNormal));
                document.Add(new Paragraph($"Cliente: {venta.Cliente?.Usuario?.Nombre}", fontNormal));
                document.Add(new Paragraph($"Documento: {venta.Cliente?.Usuario?.Identificacion}", fontNormal));
                document.Add(new Paragraph(" ", fontNormal));

                var table = new PdfPTable(4) { WidthPercentage = 100, SpacingBefore = 10, SpacingAfter = 10 };
                table.AddCell(new PdfPCell(new Phrase("Producto", fontBold)));
                table.AddCell(new PdfPCell(new Phrase("Cantidad", fontBold)));
                table.AddCell(new PdfPCell(new Phrase("Precio Unit.", fontBold)));
                table.AddCell(new PdfPCell(new Phrase("Subtotal", fontBold)));

                foreach (var detalle in venta.Detalles)
                {
                    table.AddCell(new Phrase(detalle.Producto?.Nombre, fontNormal));
                    table.AddCell(new Phrase(detalle.Cantidad.ToString(), fontNormal));
                    table.AddCell(new Phrase(detalle.PrecioUnitario.ToString("C"), fontNormal));
                    table.AddCell(new Phrase((detalle.Cantidad * detalle.PrecioUnitario).ToString("C"), fontNormal));
                }
                document.Add(table);

                var iva = venta.Total * 0.19m;
                var subtotal = venta.Total - iva;

                document.Add(new Paragraph($"Subtotal: {subtotal:C}", fontNormal) { Alignment = Element.ALIGN_RIGHT });
                document.Add(new Paragraph($"IVA (19%): {iva:C}", fontNormal) { Alignment = Element.ALIGN_RIGHT });
                document.Add(new Paragraph($"Total: {venta.Total:C}", fontBold) { Alignment = Element.ALIGN_RIGHT });

                document.Close();
            }

            return filePath; // Devolver la ruta completa
        }
    }
}
