using MailKit.Net.Smtp;
using MimeKit;

namespace AssinaturaDigital.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "sigrequisicao@gmail.com";
        private readonly string _smtpPass = "zdro mlpf bpgo dnng"; // App Password
        private readonly string _fromEmail = "sigrequisicao@gmail.com";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();

            // Nome que aparecerá no remetente
            message.From.Add(new MailboxAddress("Assinatura Digital", _fromEmail));

            // Destinatário
            message.To.Add(new MailboxAddress("", toEmail));

            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();

            // Conectar ao Gmail usando STARTTLS
            await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

            // Autenticar com usuário e senha
            await client.AuthenticateAsync(_smtpUser, _smtpPass);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
