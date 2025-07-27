using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Events
{
    public class UpdateEventStatusDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        [Range(1, 4, ErrorMessage = "ID del status es inválido")]
        public int EventStatusId { get; set; }
    }
}
