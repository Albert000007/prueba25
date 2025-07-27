using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Dance
{
    public class UpdateEventDanceTypesDto
    {
        [Required]
        public List<int> DanceTypeIds { get; set; } = new List<int>();

        public int? PrimaryDanceTypeId { get; set; } // Opcional: para marcar un baile como principal
    }
}
