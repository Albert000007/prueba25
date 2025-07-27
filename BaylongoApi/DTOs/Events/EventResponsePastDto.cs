namespace BaylongoApi.DTOs.Events
{
    public class EventResponsePastDto
    {
        public int EventId { get; set; }
        public string? MainImageUrl { get; set; }

        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? PaymentMethodId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentLink { get; set; }

        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public int EventStatusId { get; set; }
        public string EventStatus { get; set; }

        public List<ParticipantDetailDto> Participants { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public decimal Price { get; set; }

        // 👇 Nuevo campo para marcar si es evento pasado
        public bool EventPast { get; set; }
    }
}
