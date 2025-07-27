namespace BaylongoApi.DTOs.Events.Purchase
{
    public class UserPurchaseQRCodeDto
    {
        public int EventId { get; set; }
        public int PurchaseId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string QRCodeUrl { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
