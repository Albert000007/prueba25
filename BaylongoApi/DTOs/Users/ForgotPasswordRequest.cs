using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Users
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
