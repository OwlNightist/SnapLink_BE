using System;

namespace SnapLink_Repository.Entity
{
    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDelete { get; set; } = false;

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? PhotographerId { get; set; }
        public Photographer? Photographer { get; set; }

        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        public int? EventId { get; set; }
        public LocationEvent? LocationEvent { get; set; }

    }
} 