using BaylongoApi.DTOs.Stripe;

namespace BaylongoApi.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentIntentAsync(PaymentRequestDto request);
        Task SaveConfirmedPaymentAsync(PaymentConfirmationDto dto);
    }
}
