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
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _repo;

        public RatingService(IRatingRepository repo) => _repo = repo;

        public async Task<RatingDto?> GetByIdAsync(int id)
        {
            var r = await _repo.GetByIdAsync(id);
            return r == null ? null : Map(r);
        }

        public async Task<IEnumerable<RatingDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(Map);
        }

        public async Task<IEnumerable<RatingDto>> GetByPhotographerAsync(int photographerId)
        {
            var list = await _repo.GetByPhotographerAsync(photographerId);
            return list.Select(Map);
        }

        public async Task<IEnumerable<RatingDto>> GetByLocationAsync(int locationId)
        {
            var list = await _repo.GetByLocationAsync(locationId);
            return list.Select(Map);
        }

        public async Task<int> CreateAsync(CreateRatingDto dto)
        {
            ValidateTarget(dto.PhotographerId, dto.LocationId);
            ValidateScore(dto.Score);

            // 1) Booking phải tồn tại
            var booking = await _repo.GetBookingAsync(dto.BookingId)
                ?? throw new Exception("Booking không tồn tại.");

            // 2) Người đánh giá phải là user của booking
            if (booking.UserId != dto.ReviewerUserId)
                throw new Exception("Bạn không phải người đặt booking này.");

            // 3) Nếu rating cho Photographer => phải khớp với booking.PhotographerId
            if (dto.PhotographerId.HasValue && booking.PhotographerId != dto.PhotographerId.Value)
                throw new Exception("Photographer không khớp với booking.");

            // 4) Nếu rating cho Location => phải khớp với booking.LocationId
            if (dto.LocationId.HasValue && booking.LocationId != dto.LocationId.Value)
                throw new Exception("Location không khớp với booking.");

            // 5) Không cho double-rating cùng target cho cùng booking+user
            var existed = await _repo.GetExistingAsync(dto.BookingId, dto.ReviewerUserId, dto.PhotographerId, dto.LocationId);
            if (existed != null) throw new Exception("Bạn đã đánh giá rồi.");

            var rating = new Rating
            {
                BookingId = dto.BookingId,
                ReviewerUserId = dto.ReviewerUserId,
                PhotographerId = dto.PhotographerId,
                LocationId = dto.LocationId,
                Score = dto.Score,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(rating);

            // Cập nhật aggregate Photographer (nếu có)
            if (dto.PhotographerId.HasValue)
                await UpdatePhotographerAggregateOnCreate(dto.PhotographerId.Value, dto.Score);

            await _repo.SaveChangesAsync();
            return rating.RatingId;
        }

        public async Task UpdateAsync(int ratingId, UpdateRatingDto dto)
        {
            ValidateScore(dto.Score);

            var rating = await _repo.GetByIdAsync(ratingId) ?? throw new Exception("Rating không tồn tại.");
            var oldScore = rating.Score;

            rating.Score = dto.Score;
            rating.Comment = dto.Comment;
            rating.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(rating);

            // Nếu là rating cho Photographer → điều chỉnh aggregate
            if (rating.PhotographerId.HasValue)
                await UpdatePhotographerAggregateOnUpdate(rating.PhotographerId.Value, oldScore, rating.Score);

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int ratingId)
        {
            var rating = await _repo.GetByIdAsync(ratingId) ?? throw new Exception("Rating không tồn tại.");

            // Giảm aggregate nếu là Photographer
            if (rating.PhotographerId.HasValue)
                await UpdatePhotographerAggregateOnDelete(rating.PhotographerId.Value, rating.Score);

            await _repo.DeleteAsync(rating);
            await _repo.SaveChangesAsync();
        }

        // ===== Helpers =====
        private static void ValidateTarget(int? photographerId, int? locationId)
        {
            if (!photographerId.HasValue && !locationId.HasValue)
                throw new Exception("Phải chọn PhotographerId hoặc LocationId.");
            if (photographerId.HasValue && locationId.HasValue)
                throw new Exception("Chỉ được chọn 1 target: Photographer hoặc Location.");
        }

        private static void ValidateScore(byte score)
        {
            if (score < 1 || score > 5) throw new Exception("Score phải từ 1..5.");
        }

        private static RatingDto Map(Rating r) => new()
        {
            RatingId = r.RatingId,
            BookingId = r.BookingId,
            ReviewerUserId = r.ReviewerUserId,
            PhotographerId = r.PhotographerId,
            LocationId = r.LocationId,
            Score = r.Score,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };

        // --- Aggregate cho Photographer ---
        private async Task UpdatePhotographerAggregateOnCreate(int photographerId, byte newScore)
        {
            var p = await _repo.GetPhotographerAsync(photographerId) ?? throw new Exception("Photographer không tồn tại.");
            p.RatingSum = (p.RatingSum ?? 0) + newScore;
            p.RatingCount = (p.RatingCount ?? 0) + 1;
            p.Rating = p.RatingCount > 0 ? Math.Round((p.RatingSum ?? 0) / p.RatingCount.Value, 2) : 0;
        }

        private async Task UpdatePhotographerAggregateOnUpdate(int photographerId, byte oldScore, byte newScore)
        {
            var p = await _repo.GetPhotographerAsync(photographerId) ?? throw new Exception("Photographer không tồn tại.");
            p.RatingSum = (p.RatingSum ?? 0) - oldScore + newScore;
            // RatingCount giữ nguyên
            p.Rating = (p.RatingCount ?? 0) > 0 ? Math.Round((p.RatingSum ?? 0) / p.RatingCount.Value, 2) : 0;
        }

        private async Task UpdatePhotographerAggregateOnDelete(int photographerId, byte removeScore)
        {
            var p = await _repo.GetPhotographerAsync(photographerId) ?? throw new Exception("Photographer không tồn tại.");
            p.RatingSum = Math.Max(0, (p.RatingSum ?? 0) - removeScore);
            p.RatingCount = Math.Max(0, (p.RatingCount ?? 0) - 1);
            p.Rating = (p.RatingCount ?? 0) > 0 ? Math.Round((p.RatingSum ?? 0) / p.RatingCount.Value, 2) : 0;
        }
    }
}
