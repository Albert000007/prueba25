namespace BaylongoApi.DTOs.Organizations
{
    public class OrganizationInviteeDto
    {
        public Guid InvitationId { get; set; }
        public int? InvitedUserId { get; set; }
        public string InvitedUserName { get; set; }
        public string InvitedUserEmail { get; set; }
        public int InvitingUserId { get; set; }
        public string InvitingUserName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; } // Pending, Accepted, Rejected, Expired
        public string Purpose { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? AcceptedAt { get; set; }
    }
}
