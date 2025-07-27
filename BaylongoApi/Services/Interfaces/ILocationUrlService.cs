namespace BaylongoApi.Services.Interfaces
{
    public interface ILocationUrlService
    {
        string GenerateGoogleMapsUrl(decimal latitude, decimal longitude);
        string GenerateGoogleMapsUrl(string address, string city, string country);
    }
}
