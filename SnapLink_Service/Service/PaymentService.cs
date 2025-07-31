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
        private readonly ITransactionService _transactionService;

        public PaymentService(PayOS payOS, IUnitOfWork unitOfWork, SnaplinkDbContext context, IWalletService walletService, IConfiguration configuration, ITransactionService transactionService)
    {
        _payOS = payOS;
        _unitOfWork = unitOfWork;
        _context = context;
            _walletService = walletService;
            _configuration = configuration;
            _transactionService = transactionService;
    }
        // need userid for now , add to jwt token later
        private async Task<long> GenerateUniquePaymentCodeAsync()
        {
            int maxAttempts = 10;
            int attempt = 0;

            while (attempt < maxAttempts)
            {
                // Generate a more unique code using timestamp + random number
                var timestamp = DateTimeOffset.Now;
                var random = new Random();
                var randomPart = random.Next(1000, 9999); // 4-digit random number

                // Combine timestamp (6 digits) + random (4 digits) = 10 digits
                var paymentCode = long.Parse($"{timestamp.ToString("ffffff")}{randomPart}");

                // Check if this code already exists in database
                var existingPayment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.ExternalTransactionId == paymentCode.ToString());

                if (existingPayment == null)
                {
                    return paymentCode;
                }

                attempt++;
                // Small delay to ensure different timestamp
                await Task.Delay(1);
            }

            // If we can't generate a unique code after max attempts, throw exception
            throw new InvalidOperationException("Unable to generate unique payment code after multiple attempts");
        }

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
                long paymentCode = await GenerateUniquePaymentCodeAsync();
                
                // Create PayOS item data using payment amount (capped for testing)
                ItemData item = new ItemData(request.ProductName, 1, (int)paymentAmount);
            List<ItemData> items = new List<ItemData> { item };
            
                // Chỉ lấy successUrl và cancelUrl từ request, nếu không có thì báo lỗi
                if (string.IsNullOrEmpty(request.SuccessUrl) || string.IsNullOrEmpty(request.CancelUrl))
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Missing SuccessUrl or CancelUrl in request.",
                        Data = null
                    };
                }
                string successUrl = request.SuccessUrl;
                string cancelUrl = request.CancelUrl;
                
                // Create payment data
            PaymentData paymentData = new PaymentData(
                    paymentCode, 
                    (int)paymentAmount, 
                request.Description, 
                items, 
                cancelUrl, 
                successUrl
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
                Message = $"Failed to create payment link: {ex.Message}",
                Data = null
            };
        }
    }

        public async Task<PaymentResponse> GetPaymentStatusAsync(long paymentId)
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
                        Type = TransactionType.Deposit,
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
                    await _transactionService.CreatePaymentDistributionTransactionsAsync(payment.PaymentId, platformFee, photographerPayout, locationFee);

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


        public async Task<PaymentResponse> CancelPaymentAsync(int bookingId)
    {
        try
        {
            // Find payment by booking ID
            var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == bookingId);
            
            if (payment == null)
            {
                return new PaymentResponse
                {
                    Error = -1,
                    Message = "Payment not found for this booking",
                    Data = null
                };
            }

            // Cancel payment in PayOS using the external transaction ID
            PaymentLinkInformation paymentLinkInformation = await _payOS.cancelPaymentLink(long.Parse(payment.ExternalTransactionId));
            
            // Update payment status to cancelled
            payment.Status = PaymentStatus.Cancelled;
            payment.UpdatedAt = DateTime.UtcNow;
            
            // No transaction to update since we only create transactions for completed payments
            Console.WriteLine($"Payment {payment.PaymentId} cancelled - no transaction to update");
            
            // Update booking status
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking != null)
            {
                booking.Status = "Cancelled";
                booking.UpdatedAt = DateTime.UtcNow;
            }
            
            await _unitOfWork.SaveChangesAsync();

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

        public async Task HandlePayOSWebhookAsync(WebhookType payload)
    {
        // Lấy orderCode từ webhook
        var orderCode = payload.data?.orderCode;
        if (orderCode == null)
        {
            return;
        }
             
        // Tìm payment theo ExternalTransactionId (orderCode)
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.ExternalTransactionId == orderCode.ToString());
        if (payment == null)
        {
            return;
        }
        
        // Kiểm tra desc để xác định thành công (thay vì code)
        if (payload.data.desc?.ToLower() == "success")
        {
            await HandlePaymentSuccessAsync(payment);
        }
    }

        private async Task HandlePaymentSuccessAsync(Payment payment)
        {
            if (payment.Status != PaymentStatus.Success)
            {
                payment.Status = PaymentStatus.Success;
                payment.UpdatedAt = DateTime.UtcNow;
                
                // Cập nhật booking nếu cần
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == payment.BookingId);
                if (booking != null && booking.Status == "Pending")
                {
                    booking.Status = "Confirmed";
                    booking.UpdatedAt = DateTime.UtcNow;
                }
                
                await _unitOfWork.SaveChangesAsync();

                // Calculate fee distribution
                decimal platformFeePercentage = _configuration.GetValue<decimal>("PaymentSettings:PlatformFeePercentage", 10);
                decimal platformFee = payment.TotalAmount * (platformFeePercentage / 100m);
                decimal locationFee = 0m;
                decimal photographerPayout;

                // Get location info for fee calculation
                var location = await _context.Locations
                    .FirstOrDefaultAsync(l => l.LocationId == booking.LocationId);

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

                // Create distribution transactions
                await _transactionService.CreatePaymentDistributionTransactionsAsync(payment.PaymentId, platformFee, photographerPayout, locationFee);

                // Update photographer wallet using WalletService
                if (booking?.PhotographerId != null && photographerPayout > 0)
                {
                    var photographer = await _context.Photographers
                        .FirstOrDefaultAsync(p => p.PhotographerId == booking.PhotographerId);
                    
                    if (photographer != null)
                    {
                        bool success = await _walletService.AddFundsToWalletAsync(photographer.UserId, photographerPayout);
                        if (!success)
                        {
                            Console.WriteLine($"Failed to add funds to photographer wallet: UserId={photographer.UserId}, Amount={photographerPayout}");
                        }
                    }
                }

                // Update location owner wallet using WalletService (only for registered locations)
                if (booking?.LocationId != null && locationFee > 0)
                {
                    var locationWithOwner = await _context.Locations
                        .Include(l => l.LocationOwner)
                        .FirstOrDefaultAsync(l => l.LocationId == booking.LocationId);
                    
                    // Only pay location owner if it's a registered location
                    if (locationWithOwner?.LocationOwner != null && 
                        (locationWithOwner.LocationType == "Registered" || locationWithOwner.LocationType == null))
                    {
                        bool success = await _walletService.AddFundsToWalletAsync(locationWithOwner.LocationOwner.UserId, locationFee);
                        if (!success)
                        {
                            Console.WriteLine($"Failed to add funds to location owner wallet: UserId={locationWithOwner.LocationOwner.UserId}, Amount={locationFee}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Payment {payment.PaymentId} already has Success status");
            }
        }
    }
}