using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Events
{
    public class AddGalleryImageDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        public string Description { get; set; }
    }
}
