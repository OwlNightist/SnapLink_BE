using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO
{
    public class CreateRatingDto
    {
        public int BookingId { get; set; }
        public int ReviewerUserId { get; set; }
        public int? PhotographerId { get; set; }   // target 1
        public int? LocationId { get; set; }       // hoặc target 2
        public byte Score { get; set; }            // 1..5
        public string? Comment { get; set; }
    }
}
