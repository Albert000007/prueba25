using BaylongoApi.DTOs.Email;

namespace BaylongoApi.Services.Email.Contracts
{
    public interface IEmailService
    {
        Task<EmailResult> SendEmailAsync(EmailRequest request);
    }
}
