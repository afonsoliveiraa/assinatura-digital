using Microsoft.AspNetCore.Mvc;
using AssinaturaDigital.Data;
using AssinaturaDigital.Models;
using AssinaturaDigital.Services;
using Microsoft.EntityFrameworkCore;

namespace AssinaturaDigital.Controllers.Api
{
    [Route("api/attests")]
    public class AttestsApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;


        public AttestsApiController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        // POST: api/attestsApi
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Attest attest)
        {
            if (attest == null)
            {
                return Ok(new
                {
                    status = false,
                    token = "Attest não informado",
                });
            }

            if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault() ?? "Dados inválidos";

                    return Ok(new
                    {
                        status = false,
                        token = firstError
                    });
                }


            // Salva o Attest e suas Signatures em cascata
            _context.Attests.Add(attest);
            await _context.SaveChangesAsync(); // gera Ids das Signatures

            // Envia um e-mail para cada Signature
            foreach (var signature in attest.Signatures)
            {
                var toSignUrl = Url.Action(
                    action: "ToSign",
                    controller: "Signatures",
                    values: new { token = signature.Attest.Token, signature_token = signature.SignatureToken },
                    protocol: "http"
                );

                var emailBody = $@"
                    <h2>Olá {signature.Name}</h2>
                    <p>Você recebeu uma nova assinatura para o sistema <b>{attest.System}</b>.</p>
                    <p>Operação: <b>{attest.Operation}</b></p>
                    <p>Mensagem: {attest.Message}</p>
                    <p>Tipo de assinatura: {attest.TypeSignature}</p>
                    <p>Link:<a href='{toSignUrl}'>Clique aqui para assinar</a></p>
                ";

                await _emailService.SendEmailAsync(
                    toEmail: signature.Email,
                    subject: "Nova assinatura pendente",
                    body: emailBody
                );
            }

            return Ok(new
            {
                status = true,
                token = attest.Token
            });
        }


        [HttpGet("{token}")]
        public async Task<IActionResult> GetAttestSignatures(string token)
        {
            var attest = await _context.Attests
                .Include(a => a.Signatures)
                .FirstOrDefaultAsync(a => a.Token == token);

            if (attest == null)
            {
                return Ok(new
                {
                    success = false,
                    message = "Attest não encontrado",
                    attest_token = token,
                    signatures = new object[] { }
                });
            }

            var signaturesStatus = attest.Signatures
                .Select(s => new
                {
                    name = s.Name,
                    title = s.Title,
                    status = s.Status,
                    signed_at = s.SignedAt
                })
                .ToList();

            var allConfirmed = attest.Signatures.All(s => s.Status == "CONFIRMADO");

            return Ok(new
            {
                success = allConfirmed,
                message = allConfirmed
                    ? "Todas as assinaturas confirmadas"
                    : "Existem assinaturas pendentes",
                attest_token = attest.Token,
                signatures = signaturesStatus
            });
        }



    }
}
