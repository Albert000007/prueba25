namespace BaylongoApi.DTOs.Events
{
    public class EventResponseActiveDto
    {
        public int EventId { get; set; } // Existente: ID único del evento

        public string? MainImageUrl { get; set; } // Existente: URL de la imagen principal, opcional

        public int OrganizationId { get; set; } // Existente: ID de la organización

        public string OrganizationName { get; set; } // Existente: Nombre de la organización

        public string Title { get; set; } // Existente: Título del evento

        public string Description { get; set; } // Existente: Descripción del evento

        public DateTime StartDate { get; set; } // Existente: Fecha de inicio

        public DateTime EndDate { get; set; } // Existente: Fecha de fin

        public int? ContentTypeId { get; set; } // Nuevo: Identifica el tipo de evento (Evento de Baile, Clase, Curso, Taller)

        public string ContentType { get; set; } // Nuevo: Nombre del tipo de contenido

        public int? Capacity { get; set; } // Nuevo: Capacidad máxima de asistentes

        public DateTime? ExpirationDate { get; set; } // Nuevo: Fecha de caducidad para clases y cursos

        public int? Credits { get; set; } // Nuevo: Número total de créditos para clases

        public decimal BasePrice { get; set; } // Existente: Precio base, no opcional para eventos activos

        public decimal? PromotionalPrice { get; set; } // Existente: Precio promocional

        public DateTime? PromoStartDate { get; set; } // Existente: Fecha de inicio de la promoción

        public DateTime? PromoEndDate { get; set; } // Existente: Fecha de fin de la promoción

        public int? PaymentMethodId { get; set; } // Existente: ID del método de pago, opcional

        public string? PaymentMethod { get; set; } // Existente: Nombre del método de pago, opcional

        public string? PaymentLink { get; set; } // Existente: Enlace de pago, opcional

        public string? Location { get; set; } // Existente: Ubicación, opcional

        public string? Address { get; set; } // Existente: Dirección, opcional

        public string? City { get; set; } // Existente: Ciudad, opcional (a diferencia de CityId en otros DTOs)

        public string? Country { get; set; } // Existente: País, opcional

        public int EventStatusId { get; set; } // Existente: ID del estado del evento

        public string EventStatus { get; set; } // Existente: Nombre del estado

        public List<ParticipantDetailDto> Participants { get; set; } = new(); // Existente: Lista de participantes

        public DateTime CreatedAt { get; set; } // Existente: Fecha de creación

        public DateTime UpdatedAt { get; set; } // Existente: Fecha de última actualización

        public decimal? Price { get; set; } // Existente: Precio actual (base o promocional según la fecha)
    }
}
