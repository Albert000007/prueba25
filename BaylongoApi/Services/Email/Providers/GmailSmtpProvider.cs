using BaylongoApi.DTOs.Email;
using BaylongoApi.Services.Email.Contracts;
using BaylongoApi.Services.Email.Setting;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace BaylongoApi.Services.Email.Providers
{
    public class GmailSmtpProvider : IEmailProvider
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<GmailSmtpProvider> _logger;

        public string ProviderName => "Gmail";

        public GmailSmtpProvider(
            IOptions<EmailSettings> emailSettings,
            ILogger<GmailSmtpProvider> logger)
        {
            _settings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<EmailResult> SendAsync(EmailRequest request)
        {
            try
            {
                using (var client = new SmtpClient(_settings.Gmail.SmtpServer, Convert.ToInt32(_settings.Gmail.SmtpPort)))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(
                        _settings.Gmail.SmtpUsername,
                        _settings.Gmail.SmtpPassword
                    );

                    var mailMessage = new MailMessage(
                        from: new MailAddress(_settings.SenderEmail, _settings.SenderName),
                        to: new MailAddress(request.ToEmail, request.ToName)
                    )
                    {
                        Subject = request.Subject,
                        Body = request.HtmlContent,
                        IsBodyHtml = true
                    };

                    await client.SendMailAsync(mailMessage);
                    return new EmailResult(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email via Gmail SMTP");
                return new EmailResult(false, ex.Message);
            }
        }
    }
}
