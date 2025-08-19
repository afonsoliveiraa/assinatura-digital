using Microsoft.AspNetCore.Identity.UI.Services;
using AssinaturaDigital.Services;

public class IdentityEmailSender : IEmailSender
{
    private readonly EmailService _emailService;

    public IdentityEmailSender(EmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Chama o serviço de e-mail existente
        await _emailService.SendEmailAsync(email, subject, htmlMessage);
    }
}
