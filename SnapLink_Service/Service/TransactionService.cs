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
                .Include(t => t.FromUser)
                .Include(t => t.ToUser)
                .Include(t => t.ReferencePayment)
                .Where(t => t.FromUserId == userId || t.ToUserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionResponse
                {
                    TransactionId = t.TransactionId,
                    ReferencePaymentId = t.ReferencePaymentId,
                    FromUserId = t.FromUserId,
                    FromUserName = t.FromUser != null ? t.FromUser.FullName ?? "" : "System",
                    ToUserId = t.ToUserId,
                    ToUserName = t.ToUser != null ? t.ToUser.FullName ?? "" : "System",
                    Amount = t.Amount,
                    Currency = t.Currency,
                    Type = t.Type.ToString(),
                    Status = t.Status.ToString(),
                    Note = t.Note,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    PaymentMethod = t.ReferencePayment != null ? t.ReferencePayment.Method : null,
                    PaymentAmount = t.ReferencePayment != null ? t.ReferencePayment.TotalAmount : null,
                    PaymentStatus = t.ReferencePayment != null ? t.ReferencePayment.Status.ToString() : null
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
                .Include(t => t.FromUser)
                .Include(t => t.ToUser)
                .Include(t => t.ReferencePayment)
                .Where(t => t.FromUserId == photographer.User.UserId || t.ToUserId == photographer.User.UserId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionResponse
                {
                    TransactionId = t.TransactionId,
                    ReferencePaymentId = t.ReferencePaymentId,
                    FromUserId = t.FromUserId,
                    FromUserName = t.FromUser != null ? t.FromUser.FullName ?? "" : "System",
                    ToUserId = t.ToUserId,
                    ToUserName = t.ToUser != null ? t.ToUser.FullName ?? "" : "System",
                    Amount = t.Amount,
                    Currency = t.Currency,
                    Type = t.Type.ToString(),
                    Status = t.Status.ToString(),
                    Note = t.Note,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    PaymentMethod = t.ReferencePayment != null ? t.ReferencePayment.Method : null,
                    PaymentAmount = t.ReferencePayment != null ? t.ReferencePayment.TotalAmount : null,
                    PaymentStatus = t.ReferencePayment != null ? t.ReferencePayment.Status.ToString() : null
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
                .Include(t => t.FromUser)
                .Include(t => t.ToUser)
                .Include(t => t.ReferencePayment)
                .Where(t => t.FromUserId == locationOwner.User.UserId || t.ToUserId == locationOwner.User.UserId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionResponse
                {
                    TransactionId = t.TransactionId,
                    ReferencePaymentId = t.ReferencePaymentId,
                    FromUserId = t.FromUserId,
                    FromUserName = t.FromUser != null ? t.FromUser.FullName ?? "" : "System",
                    ToUserId = t.ToUserId,
                    ToUserName = t.ToUser != null ? t.ToUser.FullName ?? "" : "System",
                    Amount = t.Amount,
                    Currency = t.Currency,
                    Type = t.Type.ToString(),
                    Status = t.Status.ToString(),
                    Note = t.Note,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    PaymentMethod = t.ReferencePayment != null ? t.ReferencePayment.Method : null,
                    PaymentAmount = t.ReferencePayment != null ? t.ReferencePayment.TotalAmount : null,
                    PaymentStatus = t.ReferencePayment != null ? t.ReferencePayment.Status.ToString() : null
                })
                .ToListAsync();

            return transactions;
        }

        public async Task<TransactionResponse> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.FromUser)
                .Include(t => t.ToUser)
                .Include(t => t.ReferencePayment)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction == null)
                return null;

            return new TransactionResponse
            {
                TransactionId = transaction.TransactionId,
                ReferencePaymentId = transaction.ReferencePaymentId,
                FromUserId = transaction.FromUserId,
                FromUserName = transaction.FromUser != null ? transaction.FromUser.FullName ?? "" : "System",
                ToUserId = transaction.ToUserId,
                ToUserName = transaction.ToUser != null ? transaction.ToUser.FullName ?? "" : "System",
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                Type = transaction.Type.ToString(),
                Status = transaction.Status.ToString(),
                Note = transaction.Note,
                CreatedAt = transaction.CreatedAt,
                UpdatedAt = transaction.UpdatedAt,
                PaymentMethod = transaction.ReferencePayment != null ? transaction.ReferencePayment.Method : null,
                PaymentAmount = transaction.ReferencePayment != null ? transaction.ReferencePayment.TotalAmount : null,
                PaymentStatus = transaction.ReferencePayment != null ? transaction.ReferencePayment.Status.ToString() : null
            };
        }

        public async Task<int> GetUserTransactionCountAsync(int userId)
        {
            return await _context.Transactions
                .CountAsync(t => t.FromUserId == userId || t.ToUserId == userId);
        }

        public async Task<int> GetPhotographerTransactionCountAsync(int photographerId)
        {
            var photographer = await _context.Photographers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

            if (photographer?.User == null)
                return 0;

            return await _context.Transactions
                .CountAsync(t => t.FromUserId == photographer.User.UserId || t.ToUserId == photographer.User.UserId);
        }

        public async Task<int> GetLocationOwnerTransactionCountAsync(int locationOwnerId)
        {
            var locationOwner = await _context.LocationOwners
                .Include(p => p.User)
                .FirstOrDefaultAsync(lo => lo.LocationOwnerId== locationOwnerId);

            if (locationOwner?.User == null)
                return 0;

            return await _context.Transactions
                .CountAsync(t => t.FromUserId == locationOwner.User.UserId || t.ToUserId == locationOwner.User.UserId);
        }

        public async Task CreatePaymentDistributionTransactionsAsync(int paymentId, decimal platformFee, decimal photographerPayout, decimal locationFee)
        {
            try
            {
                // Get payment with related data
                var payment = await _context.Payments
                    .Include(p => p.Booking)
                        .ThenInclude(b => b.Photographer)
                            .ThenInclude(p => p.User)
                    .Include(p => p.Booking)
                        .ThenInclude(b => b.Location)
                            .ThenInclude(l => l.LocationOwner)
                                .ThenInclude(lo => lo.User)
                    .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

                if (payment == null)
                {
                    Console.WriteLine($"Payment not found for ID: {paymentId}");
                    return;
                }

                // Create main purchase transaction
                var purchaseTransaction = new Transaction
                {
                    ReferencePaymentId = paymentId,
                    FromUserId = payment.CustomerId,
                    ToUserId = null, // System receives the payment
                    Amount = payment.TotalAmount,
                    Type = TransactionType.Purchase,
                    Status = TransactionStatus.Success,
                    Note = $"Payment completed for booking {payment.BookingId}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(purchaseTransaction);

                // Create platform fee transaction
                var platformFeeTransaction = new Transaction
                {
                    ReferencePaymentId = paymentId,
                    FromUserId = payment.CustomerId,
                    ToUserId = null, // System receives the fee
                    Amount = platformFee,
                    Type = TransactionType.PlatformFee,
                    Status = TransactionStatus.Success,
                    Note = $"Platform commission for payment {paymentId}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(platformFeeTransaction);

                // Create photographer fee transaction
                if (photographerPayout > 0 && payment.Booking?.Photographer?.User != null)
                {
                    var photographerTransaction = new Transaction
                    {
                        ReferencePaymentId = paymentId,
                        FromUserId = payment.CustomerId,
                        ToUserId = payment.Booking.Photographer.User.UserId,
                        Amount = photographerPayout,
                        Type = TransactionType.PhotographerFee,
                        Status = TransactionStatus.Success,
                        Note = $"Photographer fee for payment {paymentId}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _context.Transactions.AddAsync(photographerTransaction);
                }

                // Create venue fee transaction (if applicable)
                if (locationFee > 0 && payment.Booking?.Location?.LocationOwner?.User != null)
                {
                    var venueTransaction = new Transaction
                    {
                        ReferencePaymentId = paymentId,
                        FromUserId = payment.CustomerId,
                        ToUserId = payment.Booking.Location.LocationOwner.User.UserId,
                        Amount = locationFee,
                        Type = TransactionType.VenueFee,
                        Status = TransactionStatus.Success,
                        Note = $"Venue fee for payment {paymentId}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _context.Transactions.AddAsync(venueTransaction);
                }

                await _context.SaveChangesAsync();

                Console.WriteLine($"Created payment distribution transactions for payment {paymentId}:");
                Console.WriteLine($"- Purchase transaction: {purchaseTransaction.TransactionId} (${payment.TotalAmount})");
                Console.WriteLine($"- Platform fee: {platformFeeTransaction.TransactionId} (${platformFee})");
                if (photographerPayout > 0)
                {
                    Console.WriteLine($"- Photographer fee: (${photographerPayout})");
                }
                if (locationFee > 0)
                {
                    Console.WriteLine($"- Venue fee: (${locationFee})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating payment distribution transactions: {ex.Message}");
                throw;
            }
        }
    }
} 