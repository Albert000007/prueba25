using BaylongoApi.DTOs.Email;
using BaylongoApi.Services.Email.Contracts;
using BaylongoApi.Services.Email.Setting;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

public class SendGridEmailProvider : IEmailProvider
{
    private readonly ISendGridClient _client;
    private readonly string _senderEmail;
    private readonly string _senderName;
    private readonly ILogger<SendGridEmailProvider> _logger;

    public string ProviderName => "SendGrid";

    public SendGridEmailProvider(
        IOptions<EmailSettings> emailSettings,
        ILogger<SendGridEmailProvider> logger)
    {
        _client = new SendGridClient(emailSettings.Value.SendGrid.ApiKey);
        _senderEmail = emailSettings.Value.SenderEmail;
        _senderName = emailSettings.Value.SenderName;
        _logger = logger;
    }

    public async Task<EmailResult> SendAsync(EmailRequest request)
    {
        try
        {
            var from = new EmailAddress(_senderEmail, _senderName);
            var to = new EmailAddress(request.ToEmail, request.ToName);
            var msg = MailHelper.CreateSingleEmail(from, to, request.Subject, null, request.HtmlContent);

            var response = await _client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Body.ReadAsStringAsync();
                _logger.LogError("SendGrid error: {StatusCode} - {ErrorResponse}",
                    response.StatusCode, errorResponse);
                return new EmailResult(false, $"SendGrid error: {response.StatusCode}");
            }

            return new EmailResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendGrid email sending failed for {Email}", request.ToEmail);
            return new EmailResult(false, ex.Message);
        }
    }
}