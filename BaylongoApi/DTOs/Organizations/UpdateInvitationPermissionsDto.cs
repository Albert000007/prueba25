using BaylongoApi.DTOs.Organizations.Enum;
using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Organizations
{
    public class UpdateInvitationPermissionsDto
    {
        [Required]
        public int RoleId { get; set; }
    }
}
