using BaylongoApi.DTOs.Email;

namespace BaylongoApi.Services.Email.Contracts
{
    public interface IEmailProvider
    {
        string ProviderName { get; }
        Task<EmailResult> SendAsync(EmailRequest request);
    }
}
