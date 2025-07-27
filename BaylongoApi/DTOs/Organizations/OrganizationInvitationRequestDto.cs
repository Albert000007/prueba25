using BaylongoApi.DTOs.Organizations.Enum;
using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Organizations
{
    public class OrganizationInvitationRequestDto
    {

        public int OrganizationId { get; set; }
        public int InvitingUserId { get; set; }

        [Required]
        [EmailAddress]
        public string? InvitedUserEmail { get; set; }

        [Required]
        public InvitationPurpose Purpose { get; set; } // Nuevo campo para el propósito

    }
}
