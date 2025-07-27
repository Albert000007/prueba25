using BaylongoApi.DTOs.Dance;

namespace BaylongoApi.DTOs.Events
{
    public class EventResponseDto
    {
        public int EventId { get; set; } // Existente: ID único del evento

        public string MainImageUrl { get; set; } // Existente: URL de la imagen principal del evento

        public int OrganizationId { get; set; } // Existente: ID de la organización que creó el evento

        public string OrganizationName { get; set; } // Existente: Nombre de la organización, para mostrar en el frontend

        public string Title { get; set; } // Existente: Título del evento

        public string Description { get; set; } // Existente: Descripción del evento

        public DateTime StartDate { get; set; } // Existente: Fecha de inicio del evento

        public DateTime EndDate { get; set; } // Existente: Fecha de fin del evento

        public int? ContentTypeId { get; set; } // Nuevo: Identifica el tipo de evento (Evento de Baile, Clase, Curso, Taller)

        public string ContentType { get; set; } // Nuevo: Nombre del tipo de contenido, para mostrar en el frontend

        public int? Capacity { get; set; } // Nuevo: Capacidad máxima de asistentes, para mostrar disponibilidad

        public DateTime? ExpirationDate { get; set; } // Nuevo: Fecha de caducidad para clases y cursos

        public int? Credits { get; set; } // Nuevo: Número total de créditos para clases

        public decimal? BasePrice { get; set; } // Existente: Precio base del evento

        public decimal? PromotionalPrice { get; set; } // Existente: Precio promocional, si aplica

        public DateTime? PromoStartDate { get; set; } // Existente: Fecha de inicio de la promoción

        public DateTime? PromoEndDate { get; set; } // Existente: Fecha de fin de la promoción

        public int PaymentMethodId { get; set; } // Existente: ID del método de pago

        public string PaymentMethod { get; set; } // Existente: Nombre del método de pago, para mostrar en el frontend

        public string PaymentLink { get; set; } // Existente: Enlace de pago, si existe

        public string Location { get; set; } // Existente: Ubicación general del evento

        public string Address { get; set; } // Existente: Dirección específica

        public int? CityId { get; set; } // Existente: ID de la ciudad

        public string Country { get; set; } // Existente: País del evento

        public int EventStatusId { get; set; } // Existente: ID del estado del evento (por ejemplo, Borrador, Publicado)

        public string EventStatus { get; set; } // Existente: Nombre del estado, para mostrar en el frontend

        public DateTime CreatedAt { get; set; } // Existente: Fecha de creación del evento

        public DateTime UpdatedAt { get; set; } // Existente: Fecha de última actualización
        public string QRCodeUrl { get; set; }
        public List<ParticipantDetailDto> Participants { get; set; } = new List<ParticipantDetailDto>(); // Existente: Lista de participantes (por ejemplo, bailarines, DJs)

        public List<DanceTypeDto> DanceTypes { get; set; } = new List<DanceTypeDto>(); // Existente: Tipos de baile asociados al evento
    }
}
