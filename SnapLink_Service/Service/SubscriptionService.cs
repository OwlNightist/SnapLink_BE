using SnapLink_Model.DTO;
using SnapLink_Repository.Constants;
using SnapLink_Repository.DBContext;
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
        private readonly IWalletRepository _wallets;
        private readonly ITransactionRepository _txRepo;
        private readonly SnaplinkDbContext _db;

        public SubscriptionService(
             IPremiumSubscriptionRepository subs,
             IWalletRepository wallets,
             ITransactionRepository txRepo,
             SnaplinkDbContext db)
        {
            _subs = subs;
            _wallets = wallets;
            _txRepo = txRepo;
            _db = db;
        }

        public async Task<SubscriptionDto> SubscribeAsync(SubscribePackageDto dto)
        {
            if (dto.PackageId <= 0) throw new Exception("PackageId không hợp lệ.");

            var pkg = await _subs.GetPackageAsync(dto.PackageId)
                      ?? throw new Exception("Package không tồn tại.");

            var duration = pkg.DurationDays ?? throw new Exception("Gói chưa cấu hình DurationDays.");
            if (pkg.Price == null || pkg.Price <= 0) throw new Exception("Giá gói không hợp lệ.");

            var applicable = (pkg.ApplicableTo ?? "").Trim();
            var now = DateTime.UtcNow;

            // Mở transaction bao toàn bộ
            await using var tx = await _db.Database.BeginTransactionAsync();

            int userIdForWallet;
            PremiumSubscription sub;

            if (applicable.Equals("Photographer", StringComparison.OrdinalIgnoreCase))
            {
                if (!dto.PhotographerId.HasValue || dto.LocationId.HasValue)
                    throw new Exception("Gói này dành cho Photographer. Chỉ truyền PhotographerId.");

                var p = await _subs.GetPhotographerAsync(dto.PhotographerId.Value)
                        ?? throw new Exception("Photographer không tồn tại.");

                userIdForWallet = p.UserId;

                // Trừ ví
                var wallet = await _wallets.GetByUserIdAsync(userIdForWallet)
                             ?? throw new Exception("Wallet không tồn tại.");
                if ((wallet.Balance ?? 0) < pkg.Price)
                    throw new Exception("Số dư ví không đủ.");
                wallet.Balance = (wallet.Balance ?? 0) - pkg.Price.Value;
                wallet.UpdatedAt = now;
                await _wallets.UpdateAsync(wallet);

                // Tính thời gian hiệu lực
                var active = await _subs.GetActiveForPhotographerAsync(p.PhotographerId);
                DateTime start = (active?.EndDate.HasValue == true && active.EndDate!.Value > now)
                    ? active.EndDate!.Value.AddSeconds(1)
                    : now;
                DateTime end = start.AddDays(duration);

                sub = new PremiumSubscription
                {
                    PackageId = pkg.PackageId,
                    PhotographerId = p.PhotographerId,
                    UserId = userIdForWallet,
                    StartDate = start,
                    EndDate = end,
                    Status = SubscriptionStatus.Active
                };

                await _subs.AddAsync(sub);

                // Log transaction
                await _txRepo.AddAsync(new Transaction
                {
                    FromUserId = userIdForWallet,                  // người trả tiền
                    ToUserId = null,                               // hoặc gán PlatformUserId nếu bạn có (VD: 1)
                    Amount = (decimal)pkg.Price!,                  // cast từ decimal? sang decimal
                    Currency = "VND",                              // giữ mặc định hoặc lấy từ pkg nếu có
                    Type = TransactionType.Purchase,               // mua gói
                    Status = TransactionStatus.Success,            // thành công
                    Note = $"Subscribe package #{pkg.PackageId} - {pkg.Name}",
                    CreatedAt = now,
                    UpdatedAt = now
                });

                // save
                await _wallets.SaveChangesAsync();
                await _subs.SaveChangesAsync();
                await _txRepo.SaveChangesAsync();
                await tx.CommitAsync();

                return Map(sub, pkg);
            }
            else if (applicable.Equals("Location", StringComparison.OrdinalIgnoreCase))
            {
                if (!dto.LocationId.HasValue || dto.PhotographerId.HasValue)
                    throw new Exception("Gói này dành cho Location. Chỉ truyền LocationId.");

                // GetLocationAsync phải Include(LocationOwner) để có UserId
                var l = await _subs.GetLocationAsync(dto.LocationId.Value)
                        ?? throw new Exception("Location không tồn tại.");

                userIdForWallet = l.LocationOwner.UserId;

                var wallet = await _wallets.GetByUserIdAsync(userIdForWallet)
                             ?? throw new Exception("Wallet không tồn tại.");
                if ((wallet.Balance ?? 0) < pkg.Price)
                    throw new Exception("Số dư ví không đủ.");
                wallet.Balance = (wallet.Balance ?? 0) - pkg.Price.Value;
                wallet.UpdatedAt = now;
                await _wallets.UpdateAsync(wallet);

                var active = await _subs.GetActiveForLocationAsync(l.LocationId);
                DateTime start = (active?.EndDate.HasValue == true && active.EndDate!.Value > now)
                    ? active.EndDate!.Value.AddSeconds(1)
                    : now;
                DateTime end = start.AddDays(duration);

                sub = new PremiumSubscription
                {
                    PackageId = pkg.PackageId,
                    LocationId = l.LocationId,
                    UserId = userIdForWallet,
                    StartDate = start,
                    EndDate = end,
                    Status = SubscriptionStatus.Active
                };

                await _subs.AddAsync(sub);

                await _txRepo.AddAsync(new Transaction
                {
                    FromUserId = userIdForWallet,                  // người trả tiền
                    ToUserId = null,                               // hoặc gán PlatformUserId nếu bạn có (VD: 1)
                    Amount = (decimal)pkg.Price!,                  // cast từ decimal? sang decimal
                    Currency = "VND",                              // giữ mặc định hoặc lấy từ pkg nếu có
                    Type = TransactionType.Purchase,               // mua gói
                    Status = TransactionStatus.Success,            // thành công
                    Note = $"Subscribe package #{pkg.PackageId} - {pkg.Name}",
                    CreatedAt = now,
                    UpdatedAt = now
                });
                await _wallets.SaveChangesAsync();
                await _subs.SaveChangesAsync();
                await _txRepo.SaveChangesAsync();
                await tx.CommitAsync();

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

        public async Task<string> CancelAsync(int subscriptionId, string? reason = null)
        {
            var sub = await _subs.GetByIdAsync(subscriptionId) ?? throw new Exception("Subscription không tồn tại.");
            if (sub.Status != SubscriptionStatus.Active) throw new Exception("Chỉ hủy được subscription đang Active.");

            sub.Status = SubscriptionStatus.Canceled;
            sub.EndDate = DateTime.UtcNow;

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
                s.Status = SubscriptionStatus.Expired;
                count++;
            }
            if (count > 0) await _subs.SaveChangesAsync();
            return count;
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
    }
}
