using BaylongoApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Events
{
    public class UpdateEventDto
    {

        [Required]
        public int OrganizationId { get; set; } // Existente: Identifica la organización que actualiza el evento, requerido para validar permisos

        [Required]
        [StringLength(100)]
        public string Title { get; set; } // Existente: Título actualizado del evento (máximo 100 caracteres), requerido

        [Required]
        public string Description { get; set; } // Existente: Descripción actualizada del evento, requerida

        [Required]
        public DateTime StartDate { get; set; } // Existente: Fecha de inicio actualizada, requerida

        [Required]
        public DateTime EndDate { get; set; } // Existente: Fecha de fin actualizada, requerida

        [Required]
        public int ContentTypeId { get; set; } // Nuevo: Tipo de evento actualizado (Evento de Baile, Clase, Curso, Taller), requerido

        public int? Capacity { get; set; } // Nuevo: Capacidad máxima actualizada, opcional para ajustar asistentes

        public DateTime? ExpirationDate { get; set; } // Nuevo: Fecha de caducidad actualizada para clases y cursos, opcional

        public int? Credits { get; set; } // Nuevo: Número de créditos actualizado para clases, opcional y requerido para clases

        public decimal? BasePrice { get; set; } // Existente: Precio base actualizado, opcional

        public decimal? PromotionalPrice { get; set; } // Existente: Precio promocional actualizado, opcional

        public DateTime? PromoStartDate { get; set; } // Existente: Fecha de inicio de la promoción actualizada, opcional

        public DateTime? PromoEndDate { get; set; } // Existente: Fecha de fin de la promoción actualizada, opcional

        [Required]
        public int PaymentMethodId { get; set; } // Existente: Método de pago actualizado, requerido

        public string PaymentLink { get; set; } // Existente: Enlace de pago actualizado, opcional

        [Required]
        public string Location { get; set; } // Existente: Ubicación actualizada, requerida

        [Required]
        public string Address { get; set; } // Existente: Dirección actualizada, requerida

        [Required]
        public string City { get; set; } // Existente: Ciudad actualizada, requerida (a diferencia de CityId en CreateEventDto)

        [Required]
        public string Country { get; set; } // Existente: País actualizado, requerido

        [Required]
        public IFormFile MainImage { get; set; } // Existente: Imagen principal actualizada, requerida para actualizar la imagen

        [ValidateParticipants]
        public List<EventParticipantDto> Participants { get; set; } = new List<EventParticipantDto>(); // Existente: Lista de participantes actualizada, opcional con validación personalizada
    }
}
