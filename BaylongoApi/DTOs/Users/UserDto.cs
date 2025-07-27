using Baylongo.Data.Data.MsSql.Models.DBBaylongo;

namespace BaylongoApi.DTOs.Users
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int UserTypeId { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string UserType { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool? IsVerified { get; set; }
        public int? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool? IsActive { get; set; }
        public UserTypeDto UserTypes { get; set; }
    }
}
