using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MedAnalyzer.Core.Application.Dto.Email;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Setting;

namespace MedAnalyzer.Infraestructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly AppSettings _appSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<MailSettings> mailSettings, IOptions<AppSettings> appSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailRequestDto emailRequest)
        {
            try
            {
                emailRequest.ToRange?.Add(emailRequest.To ?? "");
                MimeMessage email = new()
                {
                    Sender = MailboxAddress.Parse(_mailSettings.EmailFrom),
                    Subject = emailRequest.Subject
                };

                foreach (var to in emailRequest.ToRange ?? [])
                {
                    email.To.Add(MailboxAddress.Parse(to));
                }

                BodyBuilder bodyBuilder = new()
                {
                    HtmlBody = emailRequest.HtmlBody
                };

                email.Body = bodyBuilder.ToMessageBody();
                using MailKit.Net.Smtp.SmtpClient smtp = new();
                await smtp.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured {Exception}.", ex);
            }
        }

        public async Task SendPatientActivationEmailAsync(string toEmail, string firstName, string userId, string resetToken)
        {
            var activationUrl = $"{_appSettings.FrontendBaseUrl}/reset-password?userId={userId}&token={resetToken}";

            await SendEmailAsync(new EmailRequestDto
            {
                To = toEmail,
                Subject = "Acceso a tu portal de paciente — MedAnalyzer",
                HtmlBody = $@"
                    <h2>Hola, {firstName}!</h2>
                    <p>Tu médico te ha registrado en el sistema MedAnalyzer.</p>
                    <p>Haz clic en el botón de abajo para crear tu contraseña y acceder a tu portal de paciente:</p>
                    <p><a href='{activationUrl}' style='background:#0066cc;color:white;padding:10px 20px;text-decoration:none;border-radius:5px;'>
                        Activar mi cuenta
                    </a></p>
                    <p>Este enlace es válido por 12 horas.</p>
                    <p>Si no esperabas este correo, puedes ignorarlo.</p>
                    <br/>
                    <small>MedAnalyzer — Sistema de Análisis Médico</small>"
            });
        }
    }
}
