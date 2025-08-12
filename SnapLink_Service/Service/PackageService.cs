using SnapLink_Model.DTO;
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
    public class PackageService : IPackageService
    {
        private readonly IPremiumPackageRepository _repo;
        public PackageService(IPremiumPackageRepository repo) => _repo = repo;

        public async Task<int> CreateAsync(CreatePackageDto dto)
        {
            ValidateApplicable(dto.ApplicableTo);
            if (dto.Price < 0) throw new Exception("Price không hợp lệ.");
            if (dto.DurationDays <= 0) throw new Exception("DurationDays phải > 0.");

            var pkg = new PremiumPackage
            {
                ApplicableTo = dto.ApplicableTo, // "Photographer" | "Location"
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DurationDays = dto.DurationDays,
                Features = dto.Features
            };
            await _repo.AddAsync(pkg);
            await _repo.SaveChangesAsync();
            return pkg.PackageId;
        }

        public async Task UpdateAsync(int packageId, UpdatePackageDto dto)
        {
            var pkg = await _repo.GetByIdAsync(packageId) ?? throw new Exception("Package không tồn tại.");
            if (!string.IsNullOrWhiteSpace(dto.Name)) pkg.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Description)) pkg.Description = dto.Description;
            if (dto.Price.HasValue) pkg.Price = dto.Price.Value;
            if (dto.DurationDays.HasValue) pkg.DurationDays = dto.DurationDays.Value;
            if (!string.IsNullOrWhiteSpace(dto.Features)) pkg.Features = dto.Features;

            await _repo.UpdateAsync(pkg);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int packageId)
        {
            var pkg = await _repo.GetByIdAsync(packageId) ?? throw new Exception("Package không tồn tại.");
            await _repo.DeleteAsync(pkg);
            await _repo.SaveChangesAsync();
        }

        public async Task<PackageDto?> GetByIdAsync(int packageId)
        {
            var p = await _repo.GetByIdAsync(packageId);
            return p == null ? null : new PackageDto
            {
                PackageId = p.PackageId,
                ApplicableTo = p.ApplicableTo!,
                Name = p.Name!,
                Description = p.Description,
                Price = p.Price ?? 0,
                DurationDays = p.DurationDays ?? 0,
                Features = p.Features
            };
        }

        public async Task<IEnumerable<PackageDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(p => new PackageDto
            {
                PackageId = p.PackageId,
                ApplicableTo = p.ApplicableTo!,
                Name = p.Name!,
                Description = p.Description,
                Price = p.Price ?? 0,
                DurationDays = p.DurationDays ?? 0,
                Features = p.Features
            });
        }

        private static void ValidateApplicable(string applicableTo)
        {
            if (!string.Equals(applicableTo, "Photographer", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(applicableTo, "Location", StringComparison.OrdinalIgnoreCase))
                throw new Exception("ApplicableTo phải là 'Photographer' hoặc 'Location'.");
        }
    }
}
