using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Events
{
    public class UploadMainImageDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
