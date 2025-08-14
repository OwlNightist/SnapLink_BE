using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class GooglePlacesPhotoRequest
    {
        [Required]
        public string PhotoName { get; set; } = string.Empty;

        [Range(1, 4800)]
        public int? MaxHeightPx { get; set; }

        [Range(1, 4800)]
        public int? MaxWidthPx { get; set; }

        public bool SkipHttpRedirect { get; set; } = true;
    }
}
