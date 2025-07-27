using Baylongo.Data.Data.MsSql.Models.DBBaylongo;

namespace BaylongoApi.DTOs.Organizations
{
    public class ContentTypeDto
    {
        public int ContentTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
