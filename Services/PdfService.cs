using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Colors;
using iText.Kernel.Font;

namespace AssinaturaDigital.Services
{
    public class PdfService
    {
        public byte[] AddStampToPdf(string base64Pdf, string stampText)
        {
            var pdfBytes = Convert.FromBase64String(base64Pdf);

            using var ms = new MemoryStream();
            using var reader = new PdfReader(new MemoryStream(pdfBytes));
            using var writer = new PdfWriter(ms);
            using var pdfDoc = new PdfDocument(reader, writer);
            var document = new Document(pdfDoc);

            int totalPages = pdfDoc.GetNumberOfPages();

            for (int i = 1; i <= totalPages; i++)
            {
                var page = pdfDoc.GetPage(i);
                var pageSize = page.GetPageSize();

                // Coordenadas para borda direita
                float x = pageSize.GetWidth() - 20; // 20 pts da borda direita
                float y = pageSize.GetHeight() / 5; // central verticalmente

                var canvas = new PdfCanvas(page);

                // Rotaciona 90 graus e escreve
                canvas.SaveState();
                canvas.BeginText();
                canvas.SetFontAndSize(iText.Kernel.Font.PdfFontFactory.CreateFont(), 12);
                canvas.MoveText(x, y);
                canvas.SetTextMatrix(0, 1, -1, 0, x, y); // rotaciona 90° para escrever de lado
                canvas.ShowText(stampText);
                canvas.EndText();
                canvas.RestoreState();
            }

            document.Close();
            return ms.ToArray();
        }
    }
}
