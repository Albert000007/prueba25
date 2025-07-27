using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Events
{
    public class EventParticipantDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public IFormFile Photo { get; set; }  // Foto opcional

        [Required]
        [Range(1, int.MaxValue)]
        public int ParticipantTypeId { get; set; } // 1=Bailarín, 2=Academia, 3=DJ

        [Required]
        [Range(1, int.MaxValue)]
        public int RoleId { get; set; }  // Rol en el evento
    }
}
