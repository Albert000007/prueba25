namespace BaylongoApi.DTOs.Stripe
{
    public class PaymentRequestDto
    {
        public int EventId { get; set; }
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "mxn";
    }
}
