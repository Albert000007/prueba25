using BaylongoApi.DTOs.Email;
using BaylongoApi.Services.Email.Contracts;
using BaylongoApi.Services.Email.Setting;
using Microsoft.Extensions.Options;

namespace BaylongoApi.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEnumerable<IEmailProvider> _providers;
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IEnumerable<IEmailProvider> providers,
            IOptions<EmailSettings> settings,
            ILogger<EmailService> logger)
        {
            _providers = providers;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<EmailResult> SendEmailAsync(EmailRequest request)
        {
            var provider = _providers.FirstOrDefault(p =>
                p.ProviderName.Equals(_settings.Provider, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
            {
                var error = $"No provider configured for {_settings.Provider}";
                _logger.LogError(error);
                return new EmailResult(false, error);
            }

            _logger.LogInformation("Sending email using {Provider}", provider.ProviderName);
            return await provider.SendAsync(request);
        }
    }
}
