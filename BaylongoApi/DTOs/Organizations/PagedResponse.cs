namespace BaylongoApi.DTOs.Organizations
{
    public class PagedResponse<T>
    {
        public T Data { get; set; }
        public PaginationMetadata Metadata { get; set; }

        public PagedResponse(T data, PaginationMetadata metadata)
        {
            Data = data;
            Metadata = metadata;
        }
    }
}
