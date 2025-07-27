using Baylongo.Data.Data.MsSql.Models.DBBaylongo;

namespace BaylongoApi.DTOs.Organizations
{
    public class OrganizationContentPermissionDto
    {
        public int ContentTypeId { get; set; }
        public string ContentTypeName { get; set; }
        public bool? IsEnabled { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
