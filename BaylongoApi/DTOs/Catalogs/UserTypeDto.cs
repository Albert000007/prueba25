namespace BaylongoApi.DTOs.Catalogs
{
    public class UserTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? CanPublishAds { get; set; }
        public bool? CanJoinEvents { get; set; }
        public bool? RequiresOrganization { get; set; }
    }
}
