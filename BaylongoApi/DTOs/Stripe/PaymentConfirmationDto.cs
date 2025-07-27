namespace BaylongoApi.DTOs.Stripe
{
    public class PaymentConfirmationDto
    {
        public string StripePaymentIntentId { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int OrganizationId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public string Status { get; set; } = string.Empty;
        public string? ReceiptUrl { get; set; }
    }
}
