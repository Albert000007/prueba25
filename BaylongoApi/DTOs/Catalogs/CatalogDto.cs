namespace BaylongoApi.DTOs.Catalogs
{
    public class CatalogDto
    {
        public string CatalogName { get; set; }
        public IEnumerable<object> Items { get; set; }
    }
}
