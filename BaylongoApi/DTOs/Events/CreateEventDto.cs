using BaylongoApi.DTOs.City;
using BaylongoApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Events
{
    public class CreateEventDto
    {
        [Required]
        public int OrganizationId { get; set; } // Existente: Identifica la organización que crea el evento, requerido para validar permisos

        [Required]
        public string StripeAccountId { get; set; } // Existente: ID de la cuenta de Stripe para procesar pagos, requerido para transacciones

        [Required]
        [StringLength(100)]
        public string Title { get; set; } // Existente: Título del evento (máximo 100 caracteres), requerido para describir el evento

        [Required]
        public string Description { get; set; } // Existente: Descripción detallada del evento, requerida para proporcionar contexto

        [Required]
        public DateTime StartDate { get; set; } // Existente: Fecha de inicio del evento, requerida para definir el período de validez

        [Required]
        public DateTime EndDate { get; set; } // Existente: Fecha de fin del evento, requerida para establecer la duración

        [Required]
        public int ContentTypeId { get; set; } // Nuevo: Identifica el tipo de evento (Evento de Baile, Clase, Curso, Taller), requerido para clasificación

        public int? Capacity { get; set; } // Nuevo: Capacidad máxima de asistentes, opcional para limitar participantes

        public DateTime? ExpirationDate { get; set; } // Nuevo: Fecha de caducidad para clases y cursos, opcional para validar accesibilidad

        public int? Credits { get; set; } // Nuevo: Número de créditos para clases (por ejemplo, 10 clases), opcional y requerido para clases

        public decimal? BasePrice { get; set; } // Existente: Precio base del evento, opcional pero debe ser mayor a 0 si se especifica

        public decimal? PromotionalPrice { get; set; } // Existente: Precio promocional, opcional, aplicado si la compra está dentro de PromoStartDate y PromoEndDate

        public DateTime? PromoStartDate { get; set; } // Existente: Fecha de inicio de la promoción, opcional para descuentos

        public DateTime? PromoEndDate { get; set; } // Existente: Fecha de fin de la promoción, opcional para descuentos

        [Required]
        public int PaymentMethodId { get; set; } // Existente: ID del método de pago, requerido para procesar pagos

        public string PaymentLink { get; set; } // Existente: Enlace de pago generado (por ejemplo, por Stripe), opcional

        [Required]
        public string Location { get; set; } // Existente: Ubicación general del evento (por ejemplo, "Centro Cultural"), requerida

        [Required]
        public string Address { get; set; } // Existente: Dirección específica del evento, requerida

        public int? CityId { get; set; } // Existente: ID de la ciudad donde se realiza el evento, opcional

        [Required]
        public string Country { get; set; } // Existente: País del evento, requerido
    }
}
