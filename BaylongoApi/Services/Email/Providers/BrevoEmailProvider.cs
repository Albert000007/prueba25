using BaylongoApi.DTOs.Email;
using BaylongoApi.Services.Email.Contracts;
using BaylongoApi.Services.Email.Setting;
using brevo_csharp.Api;
using brevo_csharp.Client;
using brevo_csharp.Model;
using Microsoft.Extensions.Options;

namespace BaylongoApi.Services.Email.Providers
{
    public class BrevoEmailProvider : IEmailProvider
    {
        private readonly TransactionalEmailsApi _api;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly ILogger<BrevoEmailProvider> _logger;

        public string ProviderName => "Brevo";

        public BrevoEmailProvider(
            IOptions<EmailSettings> emailSettings,
            ILogger<BrevoEmailProvider> logger)
        {
            var config = new brevo_csharp.Client.Configuration();
            config.ApiKey.Add("api-key", emailSettings.Value.Brevo.ApiKey);
            _api = new TransactionalEmailsApi(config);
            _senderEmail = emailSettings.Value.SenderEmail;
            _senderName = emailSettings.Value.SenderName;
            _logger = logger;
        }

        public async Task<EmailResult> SendAsync(EmailRequest request)
        {
            try
            {

                var sender = new SendSmtpEmailSender(_senderName, _senderEmail);
                var to = new List<SendSmtpEmailTo> { new(request.ToName, request.ToEmail) };
                var email = new SendSmtpEmail(
                    sender: sender,
                    to: to,
                    subject: request.Subject,
                    htmlContent: request.HtmlContent);

                var result = await _api.SendTransacEmailAsync(email);
                return new EmailResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Brevo email sending failed for {Email}", request.ToEmail);
                return new EmailResult(false, ex.Message);
            }
        }
    }
}
