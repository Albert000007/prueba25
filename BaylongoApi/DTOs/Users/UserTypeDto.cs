namespace BaylongoApi.DTOs.Users
{
    public class UserTypeDto
    {
        public int TypeId { get; set; }

        public string TypeName { get; set; } = null!;

        public string? Description { get; set; }
    }
}
