using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssinaturaDigital.Data;
using AssinaturaDigital.Services;

namespace AssinaturaDigital.Controllers
{
    [Route("attests")]
    public class AttestsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PdfService _pdfService; // <- adiciona o service

        public AttestsController(AppDbContext context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService; // <- injeta no construtor
        }

        [HttpGet("manage/{token}")]
        public async Task<IActionResult> ManageAttest(string token)
        {
            var attest = await _context.Attests
                                       .Include(a => a.Signatures)
                                       .FirstOrDefaultAsync(a => a.Token == token);

            return View(attest);
        }
        
        [HttpPost("download-signed")]
        public IActionResult DownloadSignedPdf(string base64Pdf, string stampText)
        {
            var modifiedPdf = _pdfService.AddStampToPdf(base64Pdf, stampText);
            return File(modifiedPdf, "application/pdf", "assinatura.pdf");
        }

    }
}
