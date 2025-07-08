using System;
using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class CreatePhotographerImageRequest
    {
        [Required]
        public int PhotographerId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Caption { get; set; }

        public bool IsPrimary { get; set; } = false;
    }

    public class UpdatePhotographerImageRequest
    {
        [Required]
        public int PhotographerImageId { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [MaxLength(255)]
        public string? Caption { get; set; }

        public bool? IsPrimary { get; set; }
    }
} 