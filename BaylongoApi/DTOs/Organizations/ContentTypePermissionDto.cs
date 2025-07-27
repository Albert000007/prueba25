namespace BaylongoApi.DTOs.Organizations
{
    public class ContentTypePermissionDto
    {
        public int ContentTypeId { get; set; }
        public string Name { get; set; }
        public DateTime? PermissionGrantedAt { get; set; }
    }
}
