using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO
{
    public class CreatePackageDto
    {
        // "Photographer" hoặc "Location"
        public string ApplicableTo { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }          // VNĐ/đơn vị tiền tệ bạn dùng
        public int DurationDays { get; set; }       // Số ngày hiệu lực
        public string? Features { get; set; }       // JSON/text các tính năng
    }

    public class UpdatePackageDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? DurationDays { get; set; }
        public string? Features { get; set; }
    }

    public class PackageDto
    {
        public int PackageId { get; set; }
        public string ApplicableTo { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string? Features { get; set; }
    }

    public class SubscribePackageDto
    {
        public int PackageId { get; set; }
        // Chỉ truyền 1 trong 2: PhotographerId hoặc LocationId
        public int? PhotographerId { get; set; }
        public int? LocationId { get; set; }
    }

    public class SubscriptionDto
    {
        public int PremiumSubscriptionId { get; set; }
        public int PackageId { get; set; }
        public int? PaymentId { get; set; }
        public int? UserId { get; set; }           // nếu cần gắn user
        public int? PhotographerId { get; set; }
        public int? LocationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!;
        public string PackageName { get; set; } = null!;
        public string ApplicableTo { get; set; } = null!;
    }
}
