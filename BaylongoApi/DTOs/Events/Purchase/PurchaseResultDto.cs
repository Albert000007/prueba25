namespace BaylongoApi.DTOs.Events.Purchase
{
    public class PurchaseResultDto
    {
        public int PurchaseId { get; set; }
        public string QRCodeUrl { get; set; }
        public DateTime QRExpiresAt { get; set; }
        public EventResponseDto Event { get; set; }
    }
}
