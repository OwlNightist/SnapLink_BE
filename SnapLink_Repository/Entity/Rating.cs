using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.Entity
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int BookingId { get; set; }
        public int ReviewerUserId { get; set; }
        public int? PhotographerId { get; set; }
        public int? LocationId { get; set; }
        public byte Score { get; set; } // 1..5
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

       
        public virtual Booking Booking { get; set; } = null!;
        public virtual User ReviewerUser { get; set; } = null!;
        public virtual Photographer? Photographer { get; set; }
        public virtual Location? Location { get; set; }
    }
}
