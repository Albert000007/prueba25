using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Stripe;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace BaylongoApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly BaylongoContext _context; // Contexto de la base de datos

        public PaymentService(IConfiguration configuration, BaylongoContext context)
        {
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
            _context = context;
        }

        public async Task<PaymentResponseDto> CreatePaymentIntentAsync(PaymentRequestDto request) // Existente: Crea un intento de pago con Stripe
        {
            // Validar precio del evento
            var eventEntity = await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == request.EventId);
            if (eventEntity == null)
                throw new KeyNotFoundException("Evento no encontrado");

            var currentDate = DateTime.UtcNow;
            var expectedPrice = (eventEntity.PromoStartDate <= currentDate && currentDate <= eventEntity.PromoEndDate && eventEntity.PromotionalPrice.HasValue)
                ? eventEntity.PromotionalPrice.Value
                : eventEntity.BasePrice ?? 0;

            if (request.Amount != expectedPrice)
                throw new ArgumentException("El monto proporcionado no coincide con el precio del evento");

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100), // Stripe works with cents
                Currency = request.Currency,
                Metadata = new Dictionary<string, string>
                {
                    { "event_id", request.EventId.ToString() },
                    { "organization_id", request.OrganizationId.ToString() },
                    { "user_id", request.UserId.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return new PaymentResponseDto
            {
                ClientSecret = intent.ClientSecret
            };
        }

        public async Task SaveConfirmedPaymentAsync(PaymentConfirmationDto dto) // Existente: Guarda un pago confirmado
        {
            var exists = _context.Payments.Any(p => p.StripePaymentIntentId == dto.StripePaymentIntentId);
            if (exists)
                return; // Evitar duplicados

            var payment = new Payment
            {
                StripePaymentIntentId = dto.StripePaymentIntentId,
                UserId = dto.UserId,
                EventId = dto.EventId,
                OrganizationId = dto.OrganizationId,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Status = dto.Status,
                ReceiptUrl = dto.ReceiptUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }
    }
}
