using BaylongoApi.Services.Interfaces;
using System.Globalization;
using System.Net;

namespace BaylongoApi.Services
{
    public class LocationUrlService : ILocationUrlService
    {
        public string GenerateGoogleMapsUrl(decimal latitude, decimal longitude)
        {
            return $"https://www.google.com/maps?q={latitude.ToString(CultureInfo.InvariantCulture)},{longitude.ToString(CultureInfo.InvariantCulture)}";
        }

        public string GenerateGoogleMapsUrl(string address, string city, string country)
        {
            var encodedAddress = WebUtility.UrlEncode($"{address}, {city}, {country}");
            return $"https://www.google.com/maps?q={encodedAddress}";
        }
    }
}
