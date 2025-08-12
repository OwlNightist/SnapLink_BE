using SnapLink_Model.DTO;
using SnapLink_Repository.Constants;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IPremiumSubscriptionRepository _subs;

        public SubscriptionService(IPremiumSubscriptionRepository subs) => _subs = subs;

        public async Task<SubscriptionDto> SubscribeAsync(SubscribePackageDto dto)
        {
            if (dto.PackageId <= 0) throw new Exception("PackageId không hợp lệ.");
            var pkg = await _subs.GetPackageAsync(dto.PackageId) ?? throw new Exception("Package không tồn tại.");

            // đảm bảo có số ngày
            var duration = pkg.DurationDays ?? throw new Exception("Gói chưa cấu hình DurationDays.");

            var applicable = (pkg.ApplicableTo ?? "").Trim();

            if (applicable.Equals("Photographer", StringComparison.OrdinalIgnoreCase))
            {
                if (!dto.PhotographerId.HasValue || dto.LocationId.HasValue)
                    throw new Exception("Gói này dành cho Photographer. Chỉ truyền PhotographerId.");

                var p = await _subs.GetPhotographerAsync(dto.PhotographerId.Value)
                            ?? throw new Exception("Photographer không tồn tại.");

                var now = DateTime.UtcNow;
                var active = await _subs.GetActiveForPhotographerAsync(p.PhotographerId);

                // Nếu còn hạn → bắt đầu từ EndDate + 1s, ngược lại từ bây giờ
                DateTime start = (active?.EndDate.HasValue == true && active.EndDate!.Value > now)
                    ? active.EndDate!.Value.AddSeconds(1)
                    : now;

                DateTime end = start.AddDays(duration);

                var sub = new PremiumSubscription
                {
                    PackageId = pkg.PackageId,
                    PhotographerId = p.PhotographerId,
                    UserId = p.UserId,
                    StartDate = start,
                    EndDate = end,
                    Status = SubscriptionStatus.Active // nếu bạn đã tạo constants
                };

                await _subs.AddAsync(sub);
                await _subs.SaveChangesAsync();
                return Map(sub, pkg);
            }
            else if (applicable.Equals("Location", StringComparison.OrdinalIgnoreCase))
            {
                if (!dto.LocationId.HasValue || dto.PhotographerId.HasValue)
                    throw new Exception("Gói này dành cho Location. Chỉ truyền LocationId.");

                var l = await _subs.GetLocationAsync(dto.LocationId.Value)
                            ?? throw new Exception("Location không tồn tại.");

                var now = DateTime.UtcNow;
                var active = await _subs.GetActiveForLocationAsync(l.LocationId);

                DateTime start = (active?.EndDate.HasValue == true && active.EndDate!.Value > now)
                    ? active.EndDate!.Value.AddSeconds(1)
                    : now;

                DateTime end = start.AddDays(duration);

                var sub = new PremiumSubscription
                {
                    PackageId = pkg.PackageId,
                    LocationId = l.LocationId,
                    UserId = l.LocationOwner.UserId,
                    StartDate = start,
                    EndDate = end,
                    Status = SubscriptionStatus.Active
                };

                await _subs.AddAsync(sub);
                await _subs.SaveChangesAsync();
                return Map(sub, pkg);
            }
            else
            {
                throw new Exception("Package.ApplicableTo không hợp lệ.");
            }
        }

        public async Task<IEnumerable<SubscriptionDto>> GetByPhotographerAsync(int photographerId)
        {
            var list = await _subs.GetByPhotographerAsync(photographerId);
            return list.Select(s => Map(s, s.Package!));
        }

        public async Task<IEnumerable<SubscriptionDto>> GetByLocationAsync(int locationId)
        {
            var list = await _subs.GetByLocationAsync(locationId);
            return list.Select(s => Map(s, s.Package!));
        }

        private static SubscriptionDto Map(PremiumSubscription s, PremiumPackage pkg) => new()
        {
            PremiumSubscriptionId = s.PremiumSubscriptionId,
            PackageId = s.PackageId,
            PaymentId = s.PaymentId,
            UserId = s.UserId,
            PhotographerId = s.PhotographerId,
            LocationId = s.LocationId,
            StartDate = s.StartDate ?? DateTime.MinValue,
            EndDate = s.EndDate ?? DateTime.MinValue,
            Status = s.Status!,
            PackageName = pkg.Name!,
            ApplicableTo = pkg.ApplicableTo!
        };
        public async Task<string> CancelAsync(int subscriptionId, string? reason = null)
        {
            var sub = await _subs.GetByIdAsync(subscriptionId) ?? throw new Exception("Subscription không tồn tại.");
            if (sub.Status != "Active") throw new Exception("Chỉ hủy được subscription đang Active.");

            sub.Status = "Canceled";
            // Nếu muốn cho hiệu lực hủy ngay lập tức:
            sub.EndDate = DateTime.UtcNow;
            // (Optional) Lưu lý do hủy nếu bạn có cột (chưa có thì bỏ qua)
            // sub.CancellationReason = reason;

            await _subs.UpdateAsync(sub);
            await _subs.SaveChangesAsync();
            return "Đã hủy gói thành công.";
        }

        public async Task<int> ExpireOverduesAsync()
        {
            var now = DateTime.UtcNow;
            var list = await _subs.GetToExpireAsync(now);
            int count = 0;
            foreach (var s in list)
            {
                s.Status = "Expired";
                count++;
            }
            if (count > 0) await _subs.SaveChangesAsync();
            return count;
        }
    }
}
