using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;
using Net.payOS;
using Net.payOS.Types;
using Microsoft.Extensions.Configuration;

namespace SnapLink_Service.Service
{
public class PaymentService : IPaymentService
{
    private readonly PayOS _payOS;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SnaplinkDbContext _context;
        private readonly IWalletService _walletService;
        private readonly IConfiguration _configuration;

        public PaymentService(PayOS payOS, IUnitOfWork unitOfWork, SnaplinkDbContext context, IWalletService walletService, IConfiguration configuration)
    {
        _payOS = payOS;
        _unitOfWork = unitOfWork;
        _context = context;
            _walletService = walletService;
            _configuration = configuration;
    }
// need userid for now , add to jwt token later
    public async Task<PaymentResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request, int userId)
    {
        try
        {
                // Validate booking exists and belongs to the user
                var booking = await _context.Bookings
                    .Include(b => b.Photographer)
                        .ThenInclude(p => p.User)
                    .Include(b => b.Location)
                        .ThenInclude(l => l.LocationOwner)
                        .ThenInclude(lo => lo.User)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);
                
                if (booking == null)
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Booking not found",
                        Data = null
                    };
                }

                // Validate booking belongs to the user
                if (booking.UserId != userId)
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Booking does not belong to this user",
                        Data = null
                    };
                }

                // Validate booking has pending status
                if (booking.Status != "Pending")
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Payment can only be created for bookings with Pending status",
                        Data = null
                    };
                }

                // Check if payment already exists for this booking
                var existingPayment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == request.BookingId);
                
                if (existingPayment != null)
                {
                    // Allow creating new payment if existing payment is still pending
                    if (existingPayment.Status == PaymentStatus.Pending)
                    {
                        // Cancel the existing pending payment
                        existingPayment.Status = PaymentStatus.Cancelled;
                        existingPayment.UpdatedAt = DateTime.UtcNow;
                        await _unitOfWork.SaveChangesAsync();
                        
                        Console.WriteLine($"Cancelled existing pending payment {existingPayment.PaymentId} for booking {request.BookingId}");
                    }
                    else
                    {
                        // Don't allow new payment if existing payment is not pending
                        return new PaymentResponse
                        {
                            Error = -1,
                            Message = $"Cannot create new payment. Existing payment is in {existingPayment.Status} status",
                            Data = null
                        };
                    }
                }

                // Use booking's true price for payment calculations
                decimal truePrice = booking.TotalPrice ?? 0;
                if (truePrice <= 0)
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Invalid booking price",
                        Data = null
                    };
                }

                // For testing: hard-code payment amount at 5000
                decimal paymentAmount = 5000m;

                // Calculate payment distribution based on location type and fees
                decimal platformFeePercentage = _configuration.GetValue<decimal>("PaymentSettings:PlatformFeePercentage", 10);
                decimal platformFee = paymentAmount * (platformFeePercentage / 100m);
                decimal locationFee = 0m;
                decimal photographerPayout;

                // Determine location fee based on location type
                if (booking.Location?.LocationType == "Registered" || booking.Location?.LocationType == null) // Default to registered for backward compatibility
                {
                    locationFee = booking.Location?.HourlyRate ?? 0; // Fixed location fee for registered locations
                }
                // External locations (Google Places) have no fee

                // Calculate photographer payout
                if (locationFee > 0)
                {
                    // Registered location with fee - photographer gets remaining after platform fee and location fee
                    photographerPayout = paymentAmount - platformFee - locationFee;
                }
                else
                {
                    // External location or no location fee - photographer gets remaining after platform fee
                    photographerPayout = paymentAmount - platformFee;
                }

                // Generate unique payment code
                int paymentCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                
                // Create PayOS item data using payment amount (capped for testing)
                ItemData item = new ItemData(request.ProductName, 1, (int)paymentAmount);
            List<ItemData> items = new List<ItemData> { item };
            
                // Construct URLs using base URL from configuration
                string baseUrl = _configuration["PaymentSettings:BaseUrl"] 
                    ?? throw new InvalidOperationException("PaymentSettings:BaseUrl is not configured in appsettings.json");
                string cancelUrl = $"{baseUrl}/api/payment/cancel";
                string returnUrl = $"{baseUrl}/api/payment/success";
                
                // Create payment data
            PaymentData paymentData = new PaymentData(
                    paymentCode, 
                    (int)paymentAmount, 
                request.Description, 
                items, 
                cancelUrl, 
                returnUrl
            );

            // Create payment link with PayOS
            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

            // Create payment record with capped amount for testing
            var payment = new Payment
            {
                CustomerId = userId,
                BookingId = booking.BookingId,
                TotalAmount = paymentAmount, // Store capped amount for testing
                Status = PaymentStatus.Pending,
                ExternalTransactionId = paymentCode.ToString(),
                Method = "PayOS",
                Note = $"{request.Description} (True price: {truePrice}, Capped for testing: {paymentAmount})",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return new PaymentResponse
            {
                Error = 0,
                Message = "Payment link created successfully",
                Data = createPayment
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating payment link: {ex.Message}");
            return new PaymentResponse
            {
                Error = -1,
                Message = "Failed to create payment link",
                Data = null
            };
        }
    }

        public async Task<PaymentResponse> GetPaymentStatusAsync(int paymentId)
    {
        try
        {
            // Get payment information from PayOS
                PaymentLinkInformation paymentLinkInformation = await _payOS.getPaymentLinkInformation(paymentId);
            
            if (paymentLinkInformation.status.Equals("PAID"))
            {
                // Find the payment record
                var payment = await _context.Payments
                    .Include(p => p.Booking)
                    .ThenInclude(b => b.Photographer)
                        .ThenInclude(p => p.User)
                    .Include(p => p.Booking)
                        .ThenInclude(b => b.Location)
                        .ThenInclude(l => l.LocationOwner)
                        .ThenInclude(lo => lo.User)
                    .FirstOrDefaultAsync(p => p.ExternalTransactionId == paymentId.ToString());
                
                if (payment != null && payment.Status != PaymentStatus.Success)
                {
                    // Update payment status
                    payment.Status = PaymentStatus.Success;
                    payment.UpdatedAt = DateTime.UtcNow;
                    
                    // Create main payment transaction when payment is completed
                    var paymentTransaction = new SnapLink_Repository.Entity.Transaction
                    {
                        ReferencePaymentId = payment.PaymentId,
                        FromUserId = payment.CustomerId,
                        ToUserId = null, // System receives the payment
                        Amount = payment.TotalAmount,
                        Type = TransactionType.Payout,
                        Status = TransactionStatus.Success,
                        Note = $"Payment completed for booking {payment.BookingId}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.TransactionRepository.AddAsync(paymentTransaction);
                    Console.WriteLine($"Created payment transaction: {paymentTransaction.TransactionId}");

                    // Calculate fee distribution for transactions
                    decimal platformFee = payment.TotalAmount * (_configuration.GetValue<decimal>("PaymentSettings:PlatformFeePercentage", 10) / 100m);
                    decimal locationFee = 0m;
                    decimal photographerPayout;

                    // Get location info for fee calculation
                    var location = await _context.Locations
                        .FirstOrDefaultAsync(l => l.LocationId == payment.Booking.LocationId);

                    if (location != null && (location.LocationType == "Registered" || location.LocationType == null))
                    {
                        locationFee = location.HourlyRate ?? 0;
                    }

                    // Calculate photographer payout
                    if (locationFee > 0)
                    {
                        photographerPayout = payment.TotalAmount - platformFee - locationFee;
                    }
                    else
                    {
                        photographerPayout = payment.TotalAmount - platformFee;
                    }

                    // Create fee distribution transactions
                    await CreateFeeDistributionTransactionsAsync(payment, platformFee, photographerPayout, locationFee);

                    // Update booking status
                    if (payment.Booking != null)
                    {
                        payment.Booking.Status = "Confirmed";
                        payment.Booking.UpdatedAt = DateTime.UtcNow;
                    }

                    // Update photographer wallet
                    if (payment.Booking?.PhotographerId != null && photographerPayout > 0)
                    {
                        var photographer = await _context.Photographers
                            .FirstOrDefaultAsync(p => p.PhotographerId == payment.Booking.PhotographerId);
                        
                        if (photographer != null)
                        {
                            await _walletService.AddFundsToWalletAsync(photographer.UserId, photographerPayout);
                        }
                    }

                    // Update location owner wallet (only for registered locations)
                    if (payment.Booking?.LocationId != null && locationFee > 0)
                    {
                        var locationWithOwner = await _context.Locations
                            .Include(l => l.LocationOwner)
                            .FirstOrDefaultAsync(l => l.LocationId == payment.Booking.LocationId);
                        
                        // Only pay location owner if it's a registered location
                        if (locationWithOwner?.LocationOwner != null && 
                            (locationWithOwner.LocationType == "Registered" || locationWithOwner.LocationType == null))
                        {
                            await _walletService.AddFundsToWalletAsync(locationWithOwner.LocationOwner.UserId, locationFee);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                }
            }

            return new PaymentResponse
            {
                Error = 0,
                    Message = "Payment status retrieved successfully",
                Data = paymentLinkInformation
            };
        }
        catch (Exception ex)
        {
                Console.WriteLine($"Error getting payment status: {ex.Message}");
            return new PaymentResponse
            {
                Error = -1,
                    Message = "Failed to get payment status",
                Data = null
            };
        }
    }

        public async Task<PaymentResponse> CancelPaymentAsync(int paymentId)
    {
        try
        {
                PaymentLinkInformation paymentLinkInformation = await _payOS.cancelPaymentLink(paymentId);
            
            // Update payment status to cancelled
            var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.ExternalTransactionId == paymentId.ToString());
            
            if (payment != null)
            {
                payment.Status = PaymentStatus.Cancelled;
                payment.UpdatedAt = DateTime.UtcNow;
                
                // No transaction to update since we only create transactions for completed payments
                Console.WriteLine($"Payment {paymentId} cancelled - no transaction to update");
                
                // Update booking status
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == payment.BookingId);
                if (booking != null)
                {
                    booking.Status = "Cancelled";
                    booking.UpdatedAt = DateTime.UtcNow;
                }
                
                await _unitOfWork.SaveChangesAsync();
            }

            return new PaymentResponse
            {
                Error = 0,
                    Message = "Payment cancelled successfully",
                Data = paymentLinkInformation
            };
        }
        catch (Exception ex)
        {
                                Console.WriteLine($"Error cancelling payment: {ex.Message}");
            return new PaymentResponse
            {
                Error = -1,
                    Message = "Failed to cancel payment",
                Data = null
            };
        }
    }

        private async Task CreateFeeDistributionTransactionsAsync(Payment payment, decimal platformFee, decimal photographerPayout, decimal locationFee)
        {
            try
            {
                // Get photographer user ID
                var photographer = await _context.Photographers
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.PhotographerId == payment.Booking.PhotographerId);
                
                if (photographer?.User == null)
                {
                    Console.WriteLine($"Photographer not found for payment {payment.PaymentId}");
                    return;
                }

                // Create platform fee transaction (system transaction)
                var platformFeeTransaction = new SnapLink_Repository.Entity.Transaction
                {
                    ReferencePaymentId = payment.PaymentId,
                    FromUserId = payment.CustomerId,
                    ToUserId = null, // System receives the fee
                    Amount = platformFee,
                    Type = TransactionType.Commission,
                    Status = TransactionStatus.Success,
                    Note = $"Platform commission for payment {payment.PaymentId}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.TransactionRepository.AddAsync(platformFeeTransaction);

                // Create photographer payout transaction
                var photographerTransaction = new SnapLink_Repository.Entity.Transaction
                {
                    ReferencePaymentId = payment.PaymentId,
                    FromUserId = payment.CustomerId,
                    ToUserId = photographer.User.UserId,
                    Amount = photographerPayout,
                    Type = TransactionType.Payout,
                    Status = TransactionStatus.Success,
                    Note = $"Photographer payout for payment {payment.PaymentId}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.TransactionRepository.AddAsync(photographerTransaction);

                // Create location fee transaction (if applicable)
                SnapLink_Repository.Entity.Transaction? locationFeeTransaction = null;
                if (locationFee > 0)
                {
                    var location = await _context.Locations
                        .Include(l => l.LocationOwner)
                        .ThenInclude(lo => lo.User)
                        .FirstOrDefaultAsync(l => l.LocationId == payment.Booking.LocationId);

                    if (location?.LocationOwner?.User != null)
                    {
                        locationFeeTransaction = new SnapLink_Repository.Entity.Transaction
                        {
                            ReferencePaymentId = payment.PaymentId,
                            FromUserId = payment.CustomerId,
                            ToUserId = location.LocationOwner.User.UserId,
                            Amount = locationFee,
                            Type = TransactionType.Payout,
                            Status = TransactionStatus.Success,
                            Note = $"Location fee for payment {payment.PaymentId}",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await _unitOfWork.TransactionRepository.AddAsync(locationFeeTransaction);
                        Console.WriteLine($"Created location fee transaction: {locationFeeTransaction.TransactionId}");
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                
                Console.WriteLine($"Created fee distribution transactions for payment {payment.PaymentId}:");
                Console.WriteLine($"- Platform commission: {platformFeeTransaction.TransactionId} (${platformFee})");
                Console.WriteLine($"- Photographer payout: {photographerTransaction.TransactionId} (${photographerPayout})");
                if (locationFee > 0 && locationFeeTransaction != null)
                {
                    Console.WriteLine($"- Location fee: {locationFeeTransaction.TransactionId} (${locationFee})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating fee distribution transactions: {ex.Message}");
            }
        }
    }
}