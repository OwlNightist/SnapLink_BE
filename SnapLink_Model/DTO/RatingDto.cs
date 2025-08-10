using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO
{
    public class RatingDto
    {
        public int RatingId { get; set; }
        public int BookingId { get; set; }
        public int ReviewerUserId { get; set; }
        public int? PhotographerId { get; set; }
        public int? LocationId { get; set; }
        public byte Score { get; set; }     // 1..5
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
