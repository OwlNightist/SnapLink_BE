using Microsoft.Extensions.Configuration;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SnapLink_Service.Service
{
    public class LocationIqGeoProvider : IGeoProvider
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public LocationIqGeoProvider(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _apiKey = cfg["LocationIQ:ApiKey"] ?? throw new Exception("Missing LocationIQ:ApiKey");
            _baseUrl = cfg["LocationIQ:BaseUrl"] ?? "https://us1.locationiq.com/v1";
        }

        public async Task<(double lat, double lon)?> GeocodeAsync(string address)
        {
            var url = $"{_baseUrl}/search?key={_apiKey}&q={WebUtility.UrlEncode(address)}&format=json&limit=1";
            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return null;

            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0) return null;

            var first = doc.RootElement[0];
            var lat = double.Parse(first.GetProperty("lat").GetString()!, CultureInfo.InvariantCulture);
            var lon = double.Parse(first.GetProperty("lon").GetString()!, CultureInfo.InvariantCulture);
            return (lat, lon);
        }

    }
}
