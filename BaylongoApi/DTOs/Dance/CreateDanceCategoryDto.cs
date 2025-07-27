using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Dance
{
    public class CreateDanceCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
