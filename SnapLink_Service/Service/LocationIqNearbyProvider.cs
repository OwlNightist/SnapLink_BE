using Microsoft.Extensions.Configuration;
using SnapLink_Model.DTO.Response;
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
    public class LocationIqNearbyProvider : INearbyPoiProvider
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public LocationIqNearbyProvider(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _apiKey = cfg["LocationIQ:ApiKey"] ?? throw new Exception("Missing LocationIQ:ApiKey");
            _baseUrl = cfg["LocationIQ:BaseUrl"] ?? "https://us1.locationiq.com/v1";
        }

        public async Task<List<NearbyCombinedItem>> GetNearbyAsync(double lat, double lon, int radiusMeters, string? tags, int limit)
        {
            var url = $"{_baseUrl}/nearby?key={_apiKey}&lat={lat.ToString(CultureInfo.InvariantCulture)}&lon={lon.ToString(CultureInfo.InvariantCulture)}&radius={radiusMeters}&limit={limit}";
            if (!string.IsNullOrWhiteSpace(tags)) url += $"&tag={WebUtility.UrlEncode(tags)}";

            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return new List<NearbyCombinedItem>();

            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Array) return new List<NearbyCombinedItem>();

            var list = new List<NearbyCombinedItem>();
            foreach (var e in doc.RootElement.EnumerateArray())
            {
                // LocationIQ trả lat/lon string
                var poiLat = double.Parse(e.GetProperty("lat").GetString()!, CultureInfo.InvariantCulture);
                var poiLon = double.Parse(e.GetProperty("lon").GetString()!, CultureInfo.InvariantCulture);

                var item = new NearbyCombinedItem
                {
                    Source = "external",
                    ExternalId = e.TryGetProperty("osm_id", out var osm) ? osm.ToString() : null,
                    Class = e.TryGetProperty("class", out var c) ? c.GetString() : null,
                    Type = e.TryGetProperty("type", out var t) ? t.GetString() : null,
                    Name = e.TryGetProperty("display_name", out var dn) ? dn.GetString() : null,
                    Address = e.TryGetProperty("display_address", out var da) ? da.GetString() : null,
                    Latitude = poiLat,
                    Longitude = poiLon,
                    DistanceInKm = 0 // sẽ set ở service
                };
                list.Add(item);
            }
            return list;
        }
    }
}
