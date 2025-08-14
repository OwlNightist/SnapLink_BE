using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class GooglePlacesNearbyRequest
    {
        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        [Required]
        [Range(1, 100)]
        public int MaxResultCount { get; set; } = 10;

        [Required]
        [Range(1, 50000)]
        public double Radius { get; set; } = 500.0;

        public List<string>? IncludedTypes { get; set; }

        public string? LanguageCode { get; set; } = "vi";

        public string? FieldMask { get; set; } = 
            "places.accessibilityOptions," +
            "places.adrFormatAddress," +
            "places.attributions," +
            "places.businessStatus," +
            "places.containingPlaces," +
            "places.displayName," +
            "places.formattedAddress," +
            "places.googleMapsLinks," +
            "places.googleMapsUri," +
            "places.iconBackgroundColor," +
            "places.iconMaskBaseUri," +
            "places.id," +
            "places.location," +
            "places.name," +
            "places.photos," +
            "places.plusCode," +
            "places.postalAddress," +
            "places.primaryType," +
            "places.primaryTypeDisplayName," +
            "places.pureServiceAreaBusiness," +
            "places.shortFormattedAddress," +
            "places.types," +
            "places.utcOffsetMinutes," +
            "places.viewport";
    }
}
