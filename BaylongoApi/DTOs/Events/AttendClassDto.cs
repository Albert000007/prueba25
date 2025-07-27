using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Events
{
    public class AttendClassDto
    {
        [Required]
        public int EventId { get; set; } // Nuevo: ID del evento (clase) al que se asiste, requerido

        [Required]
        public int UserId { get; set; } // Nuevo: ID del usuario que asiste, requerido para validar inscripción

        [Required]
        public DateTime AttendanceDate { get; set; } // Nuevo: Fecha de asistencia, debe ser el día actual, requerida
    }
}
