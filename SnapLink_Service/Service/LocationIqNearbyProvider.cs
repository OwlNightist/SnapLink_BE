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

        /* public async Task<List<NearbyCombinedItem>> GetNearbyAsync(double lat, double lon, int radiusMeters, string? tags, int limit)
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
         }*/
        public async Task<List<NearbyCombinedItem>> GetNearbyAsync(double lat, double lon, int radiusMeters, string? tags, int limit)
        {
            // LocationIQ quy định radius tối đa 30000m
            radiusMeters = Math.Clamp(radiusMeters, 1, 30000);
            limit = Math.Max(1, limit);

            var url = $"{_baseUrl}/nearby?key={_apiKey}" +
                      $"&lat={lat.ToString(CultureInfo.InvariantCulture)}" +
                      $"&lon={lon.ToString(CultureInfo.InvariantCulture)}" +
                      $"&radius={radiusMeters}&limit={limit}&format=json";
            if (!string.IsNullOrWhiteSpace(tags))
                url += $"&tag={WebUtility.UrlEncode(tags)}";

            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return new List<NearbyCombinedItem>();

            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return new List<NearbyCombinedItem>();

            var list = new List<NearbyCombinedItem>();

            foreach (var e in docRootArray(doc))
            {
                // lat/lon string -> double (InvariantCulture)
                var latStr = e.GetProperty("lat").GetString()!;
                var lonStr = e.GetProperty("lon").GetString()!;
                var poiLat = double.Parse(latStr, CultureInfo.InvariantCulture);
                var poiLon = double.Parse(lonStr, CultureInfo.InvariantCulture);

                // distance (m) có sẵn trong response → convert sang km
                double? distanceKm = null;
                if (e.TryGetProperty("distance", out var distEl) && distEl.ValueKind == JsonValueKind.Number)
                {
                    var meters = distEl.GetDouble();
                    distanceKm = Math.Round(meters / 1000.0, 2);
                }

                // Một số field tên có thể khác nhau tùy POI
                string? displayName = e.TryGetProperty("display_name", out var dn) ? dn.GetString() : null;
                if (string.IsNullOrWhiteSpace(displayName) && e.TryGetProperty("name", out var nameEl))
                    displayName = nameEl.GetString();

                var item = new NearbyCombinedItem
                {
                    Source = "external",
                    ExternalId = e.TryGetProperty("osm_id", out var osm) ? osm.ToString() : null,
                    Class = e.TryGetProperty("class", out var c) ? c.GetString() : null,
                    Type = e.TryGetProperty("type", out var t) ? t.GetString() : null,
                    Name = displayName,
                    Address = e.TryGetProperty("display_address", out var da) ? da.GetString() : null,
                    Latitude = poiLat,
                    Longitude = poiLon,
                    // nếu API có trả distance thì dùng luôn, nếu không thì để 0 — service sẽ fill sau
                    DistanceInKm = distanceKm ?? 0
                };

                list.Add(item);
            }

            return list;

            static IEnumerable<JsonElement> docRootArray(JsonDocument d)
                => d.RootElement.EnumerateArray();
        }
    }
}
