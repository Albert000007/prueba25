using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Events
{
    public class AddParticipantDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public IFormFile Photo { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ParticipantTypeId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int RoleId { get; set; }
    }
}
