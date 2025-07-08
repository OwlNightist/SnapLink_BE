using System;

namespace SnapLink_Repository.Entity;

public partial class PhotographerImage
{
    public int PhotographerImageId { get; set; }
    public int PhotographerId { get; set; }
    public string? ImageUrl { get; set; }
    public string? Caption { get; set; }
    public bool? IsPrimary { get; set; }
    public DateTime? UploadedAt { get; set; }

    public virtual Photographer Photographer { get; set; } = null!;
} 