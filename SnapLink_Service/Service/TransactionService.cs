using Microsoft.EntityFrameworkCore;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly SnaplinkDbContext _context;

        public TransactionService(SnaplinkDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionResponse>> GetUserTransactionHistoryAsync(int userId, int page = 1, int pageSize = 10)
        {
            var transactions = await _context.Transactions
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionResponse
                {
                    Id = t.TransactionId,
                    WalletId = 0, // Not directly linked to wallet in current structure
                    TransactionType = t.Type ?? "Unknown",
                    Amount = t.Amount ?? 0,
                    BalanceBefore = 0, // Not stored in current structure
                    BalanceAfter = 0, // Not stored in current structure
                    Description = t.Description ?? "",
                    Status = t.Status ?? "",
                    CreatedAt = t.CreatedAt ?? DateTime.UtcNow,
                    UpdatedAt = null, // Not stored in current structure
                    UserName = t.User.FullName ?? "",
                    UserEmail = t.User.Email ?? "",
                    RelatedPaymentId = null, // Not stored in current structure
                    RelatedBookingId = null // Not stored in current structure
                })
                .ToListAsync();

            return transactions;
        }

        public async Task<IEnumerable<TransactionResponse>> GetPhotographerTransactionHistoryAsync(int photographerId, int page = 1, int pageSize = 10)
        {
            var photographer = await _context.Photographers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

            if (photographer?.User == null)
                return Enumerable.Empty<TransactionResponse>();

            var transactions = await _context.Transactions
                .Include(t => t.User)
                .Where(t => t.UserId == photographer.User.UserId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionResponse
                {
                    Id = t.TransactionId,
                    WalletId = 0, // Not directly linked to wallet in current structure
                    TransactionType = t.Type ?? "Unknown",
                    Amount = t.Amount ?? 0,
                    BalanceBefore = 0, // Not stored in current structure
                    BalanceAfter = 0, // Not stored in current structure
                    Description = t.Description ?? "",
                    Status = t.Status ?? "",
                    CreatedAt = t.CreatedAt ?? DateTime.UtcNow,
                    UpdatedAt = null, // Not stored in current structure
                    UserName = photographer.User.FullName ?? "",
                    UserEmail = photographer.User.Email ?? "",
                    RelatedPaymentId = null, // Not stored in current structure
                    RelatedBookingId = null // Not stored in current structure
                })
                .ToListAsync();

            return transactions;
        }

        public async Task<IEnumerable<TransactionResponse>> GetLocationOwnerTransactionHistoryAsync(int locationOwnerId, int page = 1, int pageSize = 10)
        {
            var locationOwner = await _context.LocationOwners
                .Include(lo => lo.User)
                .FirstOrDefaultAsync(lo => lo.LocationOwnerId == locationOwnerId);

            if (locationOwner?.User == null)
                return Enumerable.Empty<TransactionResponse>();

            var transactions = await _context.Transactions
                .Include(t => t.User)
                .Where(t => t.UserId == locationOwner.User.UserId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionResponse
                {
                    Id = t.TransactionId,
                    WalletId = 0, // Not directly linked to wallet in current structure
                    TransactionType = t.Type ?? "Unknown",
                    Amount = t.Amount ?? 0,
                    BalanceBefore = 0, // Not stored in current structure
                    BalanceAfter = 0, // Not stored in current structure
                    Description = t.Description ?? "",
                    Status = t.Status ?? "",
                    CreatedAt = t.CreatedAt ?? DateTime.UtcNow,
                    UpdatedAt = null, // Not stored in current structure
                    UserName = locationOwner.User.FullName ?? "",
                    UserEmail = locationOwner.User.Email ?? "",
                    RelatedPaymentId = null, // Not stored in current structure
                    RelatedBookingId = null // Not stored in current structure
                })
                .ToListAsync();

            return transactions;
        }

        public async Task<TransactionResponse> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction == null)
                return null;

            return new TransactionResponse
            {
                Id = transaction.TransactionId,
                WalletId = 0, // Not directly linked to wallet in current structure
                TransactionType = transaction.Type ?? "Unknown",
                Amount = transaction.Amount ?? 0,
                BalanceBefore = 0, // Not stored in current structure
                BalanceAfter = 0, // Not stored in current structure
                Description = transaction.Description ?? "",
                Status = transaction.Status ?? "",
                CreatedAt = transaction.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = null, // Not stored in current structure
                UserName = transaction.User.FullName ?? "",
                UserEmail = transaction.User.Email ?? "",
                RelatedPaymentId = null, // Not stored in current structure
                RelatedBookingId = null // Not stored in current structure
            };
        }

        public async Task<int> GetUserTransactionCountAsync(int userId)
        {
            return await _context.Transactions
                .CountAsync(t => t.UserId == userId);
        }

        public async Task<int> GetPhotographerTransactionCountAsync(int photographerId)
        {
            var photographer = await _context.Photographers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

            if (photographer?.User == null)
                return 0;

            return await _context.Transactions
                .CountAsync(t => t.UserId == photographer.User.UserId);
        }

        public async Task<int> GetLocationOwnerTransactionCountAsync(int locationOwnerId)
        {
            var locationOwner = await _context.LocationOwners
                .Include(p => p.User)
                .FirstOrDefaultAsync(lo => lo.LocationOwnerId== locationOwnerId);

            if (locationOwner?.User == null)
                return 0;

            return await _context.Transactions
                .CountAsync(t => t.UserId == locationOwner.User.UserId);
        }
    }
} 