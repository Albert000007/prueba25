namespace BaylongoApi.DTOs.Organizations
{
    public class OrganizationInvitationResponseDto
    {
        public Guid InvitationId { get; set; }
        public string Status { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
