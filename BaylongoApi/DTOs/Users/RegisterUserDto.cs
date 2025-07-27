using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Users
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(600, MinimumLength = 6)]
        public string? Password { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public int UserTypeId { get; set; }

        // Solo requerido si es organizador
        public int? OrganizationId { get; set; }
    }
}
