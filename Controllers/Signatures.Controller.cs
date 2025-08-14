using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssinaturaDigital.Data;
using AssinaturaDigital.Services;

namespace AssinaturaDigital.Controllers
{
    [Route("signatures")]
    public class SignaturesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;


        public SignaturesController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

        }

        // GET: /signatures/:token_attest + :signature_token
        [HttpGet("{token}+{signature_token}")]
        public async Task<IActionResult> ToSign(string token, string signature_token)
        {
            var signature = await _context.Signatures
                                    .Include(s => s.Attest)
                                    .FirstOrDefaultAsync(s => s.SignatureToken == signature_token);

            return View(signature);
        }

        // GET: /signatures/edit-email/{id}
        [HttpGet("edit-email/{id}")]
        public async Task<IActionResult> EditEmail(int id)
        {
            var signature = await _context.Signatures
                                    .Include(s => s.Attest)
                                    .FirstOrDefaultAsync(s => s.Id == id);

            return PartialView("_EditEmailPartial", signature);
        }

        [HttpPost("edit-email/{id}")]
        public async Task<IActionResult> EditEmail(int id, [FromForm] string email)
        {
            var signature = await _context.Signatures
                                        .Include(s => s.Attest)
                                        .FirstOrDefaultAsync(s => s.Id == id);

            if (signature == null) {
                return NotFound();
            }

            signature.Email = email;

            // Salva no banco
            await _context.SaveChangesAsync();

            // Gera o link absoluto para a action ToSign
            var toSignUrl = Url.Action(
                action: "ToSign",
                controller: "Signatures",
                values: new { token = signature.Attest.Token, signature_token = signature.SignatureToken },
                protocol: Request.Scheme // garante http ou https correto
            );

            var emailBody = $@"
                <h2>Olá {signature.Name}</h2>
                <p>Você recebeu uma nova assinatura para o sistema <b>{signature.Attest.System}</b>.</p>
                <p>Operação: <b>{signature.Attest.Operation}</b></p>
                <p>Mensagem: {signature.Attest.Message}</p>
                <p>Tipo de assinatura: {signature.Attest.TypeSignature}</p>

                <p><a href='{toSignUrl}'>Clique aqui para assinar</a></p>
            ";

            await _emailService.SendEmailAsync(
                toEmail: signature.Email,
                subject: "Nova assinatura pendente",
                body: emailBody
            );

            // Aqui é o equivalente ao flash[:notice] do Rails
            TempData["ToastMessage"] = "E-mail atualizado com sucesso!";

            // Redireciona para o Attest usando o token
            return RedirectToAction(
                actionName: nameof(AttestsController.ManageAttest),
                controllerName: "Attests",
                routeValues: new { token = signature.Attest?.Token }
            );
        }

        [HttpPost("edit-status/{id}")]
        public async Task<IActionResult> EditStatus(int id)
        {
            var signature = await _context.Signatures
                                        .Include(s => s.Attest)
                                        .FirstOrDefaultAsync(s => s.Id == id);

            if (signature == null)
                return NotFound();

            signature.Status = "CONFIRMADO";
            signature.SignedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Redireciona para ToSign usando token + signature_token
            return RedirectToAction("ToSign", new { 
                token = signature.Attest.Token, 
                signature_token = signature.SignatureToken 
            });
        }

    }

}