using Microsoft.EntityFrameworkCore;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SnapLink_Service.Service
{
    public class EscrowService : IEscrowService
    {
        private readonly SnaplinkDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletService _walletService;
        private readonly IPaymentCalculationService _paymentCalculationService;

        public EscrowService(SnaplinkDbContext context, IUnitOfWork unitOfWork, IWalletService walletService, IPaymentCalculationService paymentCalculationService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _walletService = walletService;
            _paymentCalculationService = paymentCalculationService;
        }

        public async Task<bool> HoldFundsInEscrowAsync(int bookingId, int userId, decimal amount)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Photographer)
                    .Include(b => b.Location)
                        .ThenInclude(l => l.LocationOwner)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    Console.WriteLine($"Booking {bookingId} not found");
                    return false;
                }

                // Create escrow hold transaction
                var escrowHoldTransaction = new Transaction
                {
                    FromUserId = userId,
                    ToUserId = null, // System holds the funds
                    Amount = amount,
                    Type = TransactionType.EscrowHold,
                    Status = TransactionStatus.Held,
                    Note = $"Funds held in escrow for booking {bookingId}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(escrowHoldTransaction);
                await _unitOfWork.SaveChangesAsync();

                Console.WriteLine($"Funds held in escrow for booking {bookingId}: ${amount}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error holding funds in escrow: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ReleaseFundsFromEscrowAsync(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Photographer)
                    .Include(b => b.Location)
                        .ThenInclude(l => l.LocationOwner)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    Console.WriteLine($"Booking {bookingId} not found");
                    return false;
                }

                // Calculate payment distribution
                var calculationResult = await _paymentCalculationService.CalculatePaymentDistributionAsync(booking.TotalPrice ?? 0, booking.Location, booking.BookingId);
                var vnCulture = CultureInfo.GetCultureInfo("vi-VN");

                // Create escrow release transaction
                var escrowReleaseTransaction = new Transaction
                {
                    FromUserId = null, // System releases funds
                    ToUserId = null, // System releases funds
                    Amount = booking.TotalPrice ?? 0,
                    Type = TransactionType.EscrowRelease,
                    Status = TransactionStatus.Released,
                    Note = $"Funds released from escrow for booking {Regex.Replace(calculationResult.CalculationNote, @"\$(\d+(?:\.\d+)?)", m => string.Format(vnCulture, "{0:N0} VND", decimal.Parse(m.Groups[1].Value)))}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(escrowReleaseTransaction);

                // Distribute funds to photographer
                if (calculationResult.PhotographerPayout > 0 && booking.Photographer != null)
                {
                    var photographerTransaction = new Transaction
                    {
                        FromUserId = null, // System releases funds
                        ToUserId = booking.Photographer.UserId,
                        Amount = calculationResult.PhotographerPayout,
                        Type = TransactionType.PhotographerFee,
                        Status = TransactionStatus.Success,
                        Note = $"phí cho thợ chụp từ dịch vụ: book {bookingId}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _context.Transactions.AddAsync(photographerTransaction);
                    await _walletService.AddFundsToWalletAsync(booking.Photographer.UserId, calculationResult.PhotographerPayout);
                }

                // Distribute funds to location owner (only for registered locations)
                if (calculationResult.LocationFee > 0 && booking.Location?.LocationOwner != null && 
                    (booking.Location.LocationType == "Registered" || booking.Location.LocationType == null))
                {
                    var venueTransaction = new Transaction
                    {
                        FromUserId = null, // System releases funds
                        ToUserId = booking.Location.LocationOwner.UserId,
                        Amount = calculationResult.LocationFee,
                        Type = TransactionType.VenueFee,
                        Status = TransactionStatus.Success,
                        Note = $"phí cho địa điểm từ dịch vụ: book {bookingId}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _context.Transactions.AddAsync(venueTransaction);
                    await _walletService.AddFundsToWalletAsync(booking.Location.LocationOwner.UserId, calculationResult.LocationFee);
                }

                // Platform fee transaction
                var platformFeeTransaction = new Transaction
                {
                    FromUserId = null, // System receives the fee
                    ToUserId = null, // System receives the fee
                    Amount = calculationResult.PlatformFee,
                    Type = TransactionType.PlatformFee,
                    Status = TransactionStatus.Success,
                    Note = $"Platform fee from escrow for booking {bookingId}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(platformFeeTransaction);

                await _unitOfWork.SaveChangesAsync();

                Console.WriteLine($"Funds released from escrow for booking {bookingId}: ${booking.TotalPrice}");
                Console.WriteLine($"- Photographer: ${calculationResult.PhotographerPayout}");
                Console.WriteLine($"- Location owner: ${calculationResult.LocationFee}");
                Console.WriteLine($"- Platform fee: ${calculationResult.PlatformFee}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error releasing funds from escrow: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RefundFundsFromEscrowAsync(int bookingId, int userId)
        {
            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    Console.WriteLine($"Booking {bookingId} not found");
                    return false;
                }

                // Create escrow refund transaction
                var escrowRefundTransaction = new Transaction
                {
                    FromUserId = null, // System refunds funds
                    ToUserId = userId,
                    Amount = booking.TotalPrice ?? 0,
                    Type = TransactionType.EscrowRefund,
                    Status = TransactionStatus.Success,
                    Note = $"Funds refunded from escrow for cancelled booking {bookingId}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(escrowRefundTransaction);

                // Add funds back to user's wallet
                await _walletService.AddFundsToWalletAsync(userId, booking.TotalPrice ?? 0);

                await _unitOfWork.SaveChangesAsync();

                Console.WriteLine($"Funds refunded from escrow for booking {bookingId}: ${booking.TotalPrice} to user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refunding funds from escrow: {ex.Message}");
                return false;
            }
        }

        public async Task<decimal> GetEscrowBalanceAsync(int bookingId)
        {
            try
            {
                var escrowTransactions = await _context.Transactions
                    .Where(t => t.Note.Contains($"booking {bookingId}") && 
                               (t.Type == TransactionType.EscrowHold || 
                                t.Type == TransactionType.EscrowRelease || 
                                t.Type == TransactionType.EscrowRefund))
                    .ToListAsync();

                decimal balance = 0;
                foreach (var transaction in escrowTransactions)
                {
                    if (transaction.Type == TransactionType.EscrowHold && transaction.Status == TransactionStatus.Held)
                    {
                        balance += transaction.Amount;
                    }
                    else if (transaction.Type == TransactionType.EscrowRelease || transaction.Type == TransactionType.EscrowRefund)
                    {
                        balance -= transaction.Amount;
                    }
                }

                return balance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting escrow balance: {ex.Message}");
                return 0;
            }
        }

        public async Task<IEnumerable<TransactionResponse>> GetEscrowTransactionsAsync(int bookingId)
        {
            try
            {
                var escrowTransactions = await _context.Transactions
                    .Include(t => t.FromUser)
                    .Include(t => t.ToUser)
                    .Where(t => t.Note.Contains($"booking {bookingId}") && 
                               (t.Type == TransactionType.EscrowHold || 
                                t.Type == TransactionType.EscrowRelease || 
                                t.Type == TransactionType.EscrowRefund ||
                                t.Type == TransactionType.PhotographerFee ||
                                t.Type == TransactionType.VenueFee ||
                                t.Type == TransactionType.PlatformFee))
                    .OrderBy(t => t.CreatedAt)
                    .Select(t => new TransactionResponse
                    {
                        TransactionId = t.TransactionId,
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
                        UpdatedAt = t.UpdatedAt
                    })
                    .ToListAsync();

                return escrowTransactions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting escrow transactions: {ex.Message}");
                return Enumerable.Empty<TransactionResponse>();
            }
        }
    }
} 