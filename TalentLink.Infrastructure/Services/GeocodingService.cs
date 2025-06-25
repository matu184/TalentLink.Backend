using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TalentLink.Infrastructure.Services
{
    public class GeocodingService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public GeocodingService(IConfiguration config)
        {
            _http = new HttpClient();
            _apiKey = config["OpenCage:ApiKey"]!;
        }

        public async Task<(double lat, double lng)> GetCoordinatesAsync(string zip, string city)
        {
            var query = $"{zip}, {city}";
            var url = $"https://api.opencagedata.com/geocode/v1/json?q={query}&key={_apiKey}";
<<<<<<< HEAD
            Console.WriteLine($"Geocoding-Request: zip='{zip}', city='{city}'");
=======

>>>>>>> heroku/main
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) throw new Exception("Geocoding fehlgeschlagen");

            using var stream = await response.Content.ReadAsStreamAsync();
            var json = await JsonDocument.ParseAsync(stream);
            var results = json.RootElement.GetProperty("results");
<<<<<<< HEAD
            if (results.GetArrayLength() == 0)
            {
                throw new Exception($"Kein Geocoding-Ergebnis für '{query}' gefunden.");
            }
=======
>>>>>>> heroku/main
            var geometry = results[0].GetProperty("geometry");

            var lat = geometry.GetProperty("lat").GetDouble();
            var lng = geometry.GetProperty("lng").GetDouble();

            return (lat, lng);
        }
    }
}
