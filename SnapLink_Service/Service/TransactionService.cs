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

        public async Task<IEnumerable<TransactionResponse>> GetUserTransactionHistoryAsync(int userId, int page = 1, int pageSize = 10, int? year = null, int? month = null)
        {
            var query = _context.Transactions
                .Include(t => t.FromUser)
                .Include(t => t.ToUser)
                .Include(t => t.ReferencePayment)
                .Where(t => t.FromUserId == userId || t.ToUserId == userId);

            // Apply year and month filter if provided
            if (year.HasValue && month.HasValue)
            {
                var startDate = new DateTime(year.Value, month.Value, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                query = query.Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);
            }

            var transactions = await query
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
                    PaymentStatus = t.ReferencePayment != null ? t.ReferencePayment.Status.ToString() : null
                })
                .ToListAsync();

            return transactions;
        }

        public async Task<IEnumerable<TransactionResponse>> GetPhotographerTransactionHistoryAsync(int photographerId, int page = 1, int pageSize = 10, int? year = null, int? month = null)
        {
            var photographer = await _context.Photographers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

            if (photographer?.User == null)
                return Enumerable.Empty<TransactionResponse>();

            var query = _context.Transactions
                .Include(t => t.FromUser)
                .Include(t => t.ToUser)
                .Include(t => t.ReferencePayment)
                .Where(t => t.FromUserId == photographer.User.UserId || t.ToUserId == photographer.User.UserId);

            // Apply year and month filter if provided
            if (year.HasValue && month.HasValue)
            {
                var startDate = new DateTime(year.Value, month.Value, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                query = query.Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);
            }

            var transactions = await query
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
                    PaymentStatus = t.ReferencePayment != null ? t.ReferencePayment.Status.ToString() : null
                })
                .ToListAsync();

            return transactions;
        }

        public async Task<IEnumerable<TransactionResponse>> GetLocationOwnerTransactionHistoryAsync(int locationOwnerId, int page = 1, int pageSize = 10, int? year = null, int? month = null)
        {
            var locationOwner = await _context.LocationOwners
                .Include(lo => lo.User)
                .FirstOrDefaultAsync(lo => lo.LocationOwnerId == locationOwnerId);

            if (locationOwner?.User == null)
                return Enumerable.Empty<TransactionResponse>();

            var query = _context.Transactions
                .Include(t => t.FromUser)
                .Include(t => t.ToUser)
                .Include(t => t.ReferencePayment)
                .Where(t => t.FromUserId == locationOwner.User.UserId || t.ToUserId == locationOwner.User.UserId);

            // Apply year and month filter if provided
            if (year.HasValue && month.HasValue)
            {
                var startDate = new DateTime(year.Value, month.Value, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                query = query.Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);
            }

            var transactions = await query
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
                PaymentStatus = transaction.ReferencePayment != null ? transaction.ReferencePayment.Status.ToString() : null
            };
        }

        public async Task<int> GetUserTransactionCountAsync(int userId, int? year = null, int? month = null)
        {
            var query = _context.Transactions
                .Where(t => t.FromUserId == userId || t.ToUserId == userId);

            // Apply year and month filter if provided
            if (year.HasValue && month.HasValue)
            {
                var startDate = new DateTime(year.Value, month.Value, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                query = query.Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetPhotographerTransactionCountAsync(int photographerId, int? year = null, int? month = null)
        {
            var photographer = await _context.Photographers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

            if (photographer?.User == null)
                return 0;

            var query = _context.Transactions
                .Where(t => t.FromUserId == photographer.User.UserId || t.ToUserId == photographer.User.UserId);

            // Apply year and month filter if provided
            if (year.HasValue && month.HasValue)
            {
                var startDate = new DateTime(year.Value, month.Value, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                query = query.Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetLocationOwnerTransactionCountAsync(int locationOwnerId, int? year = null, int? month = null)
        {
            var locationOwner = await _context.LocationOwners
                .Include(p => p.User)
                .FirstOrDefaultAsync(lo => lo.LocationOwnerId== locationOwnerId);

            if (locationOwner?.User == null)
                return 0;

            var query = _context.Transactions
                .Where(t => t.FromUserId == locationOwner.User.UserId || t.ToUserId == locationOwner.User.UserId);

            // Apply year and month filter if provided
            if (year.HasValue && month.HasValue)
            {
                var startDate = new DateTime(year.Value, month.Value, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                query = query.Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);
            }

            return await query.CountAsync();
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

        public async Task<MonthlyIncomeResponse> GetMonthlyIncomeAsync(int userId, int year, int month)
        {
            try
            {
                // Calculate date range for the specified month
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                // Get all transactions for the user in the specified month
                var transactions = await _context.Transactions
                    .Include(t => t.FromUser)
                    .Include(t => t.ToUser)
                    .Include(t => t.ReferencePayment)
                    .Where(t => (t.FromUserId == userId || t.ToUserId == userId) &&
                               t.CreatedAt >= startDate && t.CreatedAt <= endDate &&
                               t.Status == TransactionStatus.Success)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                var response = new MonthlyIncomeResponse
                {
                    UserId = userId,
                    Year = year,
                    Month = month,
                    TotalTransactions = transactions.Count
                };

                // Calculate income and expense
                foreach (var transaction in transactions)
                {
                    if (transaction.ToUserId == userId)
                    {
                        // Income: Money received by the user
                        response.TotalIncome += transaction.Amount;
                        
                        // Group by transaction type
                        var typeKey = transaction.Type.ToString();
                        if (!response.IncomeByType.ContainsKey(typeKey))
                            response.IncomeByType[typeKey] = 0;
                        response.IncomeByType[typeKey] += transaction.Amount;
                    }
                    else if (transaction.FromUserId == userId)
                    {
                        // Expense: Money paid by the user
                        response.TotalExpense += transaction.Amount;
                    }
                }

                response.NetIncome = response.TotalIncome - response.TotalExpense;

                // Get recent transactions (last 10)
                response.RecentTransactions = transactions
                    .Take(10)
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
                        PaymentStatus = t.ReferencePayment != null ? t.ReferencePayment.Status.ToString() : null
                    })
                    .ToList();

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting monthly income: {ex.Message}");
                throw;
            }
        }
    }
} 