using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO
{
    public class SubscriptionLiteDto
    {
        public int PremiumSubscriptionId { get; set; }
        public int PackageId { get; set; }
        public string? PackageName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
    }
}
