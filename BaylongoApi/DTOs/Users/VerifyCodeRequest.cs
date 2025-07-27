using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Users
{
    public class VerifyCodeRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
