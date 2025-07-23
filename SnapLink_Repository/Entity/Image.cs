using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Image
{
    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsPrimary { get; set; }
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? PhotographerId { get; set; }
        public Photographer? Photographer { get; set; }

        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        public int? PhotographerEventId { get; set; }
        public PhotographerEvent? PhotographerEvent { get; set; }
    }
} 