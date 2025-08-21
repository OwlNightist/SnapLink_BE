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
    private readonly IPaymentCalculationService _paymentCalculationService;
    private readonly IPushNotificationService _pushNotificationService;

    public PaymentService(PayOS payOS, IUnitOfWork unitOfWork, SnaplinkDbContext context, IWalletService walletService, IConfiguration configuration, ITransactionService transactionService, IPaymentCalculationService paymentCalculationService, IPushNotificationService pushNotificationService)
    {
        _payOS = payOS;
        _unitOfWork = unitOfWork;
        _context = context;
        _walletService = walletService;
        _configuration = configuration;
        _transactionService = transactionService;
        _paymentCalculationService = paymentCalculationService;
        _pushNotificationService = pushNotificationService;
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
                    .FirstOrDefaultAsync(p => p.BookingId == request.BookingId && p.BookingId.HasValue);
                
                if (existingPayment != null)
                {
                    if (existingPayment.Status == PaymentStatus.Pending)
                    {
                        existingPayment.Status = PaymentStatus.Cancelled;
                        existingPayment.UpdatedAt = DateTime.UtcNow;
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new PaymentResponse
                        {
                            Error = -1,
                            Message = $"Cannot create new payment. Existing payment is in {existingPayment.Status} status",
                            Data = null
                        };
                    }
                }

                // Validate booking price
                decimal paymentAmount = booking.TotalPrice ?? 0;
                if (paymentAmount <= 0)
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Invalid booking price",
                        Data = null
                    };
                }

                // Validate URLs
                if (string.IsNullOrEmpty(request.SuccessUrl) || string.IsNullOrEmpty(request.CancelUrl))
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Missing SuccessUrl or CancelUrl in request.",
                        Data = null
                    };
                }

                // Generate unique payment code
                long paymentCode = await GenerateUniquePaymentCodeAsync();
                
                // Create PayOS item data
                var item = new ItemData(request.ProductName, 1, (int)paymentAmount);
                var items = new List<ItemData> { item };
                
                // Create payment data
                var paymentData = new PaymentData(
                    paymentCode, 
                    (int)paymentAmount, 
                    request.Description, 
                    items, 
                    request.CancelUrl, 
                    request.SuccessUrl
                );

                // Create payment link with PayOS
                var createPayment = await _payOS.createPaymentLink(paymentData);

                // Create payment record
                var payment = new Payment
                {
                    CustomerId = userId,
                    BookingId = booking.BookingId,
                    TotalAmount = paymentAmount,
                    Status = PaymentStatus.Pending,
                    ExternalTransactionId = paymentCode.ToString(),
                    Method = "PayOS",
                    Note = request.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                // Create response data that includes both PayOS data and our payment ID
                var responseData = new
                {
                    PaymentId = payment.PaymentId,
                    PayOSData = createPayment
                };

                return new PaymentResponse
                {
                    Error = 0,
                    Message = "Payment link created successfully",
                    Data = responseData
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

        public async Task<PaymentResponse> CreateWalletTopUpLinkAsync(CreateWalletTopUpRequest request, int userId)
        {
            try
            {
                // Validate amount
                if (request.Amount <= 0)
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Invalid amount",
                        Data = null
                    };
                }

                // Validate URLs
                if (string.IsNullOrEmpty(request.SuccessUrl) || string.IsNullOrEmpty(request.CancelUrl))
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Missing SuccessUrl or CancelUrl in request.",
                        Data = null
                    };
                }

                // Generate unique payment code
                long paymentCode = await GenerateUniquePaymentCodeAsync();
                
                // Create PayOS item data
                var item = new ItemData(request.ProductName, 1, (int)request.Amount);
                var items = new List<ItemData> { item };
                
                // Create payment data
                var paymentData = new PaymentData(
                    paymentCode, 
                    (int)request.Amount, 
                    request.Description, 
                    items, 
                    request.CancelUrl, 
                    request.SuccessUrl
                );

                // Create payment link with PayOS
                var createPayment = await _payOS.createPaymentLink(paymentData);

                // Create payment record for wallet top-up (no booking)
                var payment = new Payment
                {
                    CustomerId = userId,
                    BookingId = null, // No booking for wallet top-up
                    TotalAmount = request.Amount,
                    Status = PaymentStatus.Pending,
                    ExternalTransactionId = paymentCode.ToString(),
                    Method = "PayOS",
                    Note = $"Wallet top-up: {request.Description}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                // Create response data that includes both PayOS data and our payment ID
                var responseData = new
                {
                    PaymentId = payment.PaymentId,
                    PayOSData = createPayment
                };

                return new PaymentResponse
                {
                    Error = 0,
                    Message = "Wallet top-up payment link created successfully",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating wallet top-up payment link: {ex.Message}");
                return new PaymentResponse
                {
                    Error = -1,
                    Message = $"Failed to create wallet top-up payment link: {ex.Message}",
                    Data = null
                };
            }
        }

        /*
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
                            Note = payment.BookingId.HasValue ? $"Payment completed for booking {payment.BookingId}" : "Wallet top-up payment completed",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await _unitOfWork.TransactionRepository.AddAsync(paymentTransaction);
                        Console.WriteLine($"Created payment transaction: {paymentTransaction.TransactionId}");

                        // Handle booking payments vs wallet top-ups
                        if (payment.BookingId.HasValue && payment.Booking != null)
                        {
                            // This is a booking payment - distribute funds to photographer and location owner
                            var calculationResult = await _paymentCalculationService.CalculatePaymentDistributionAsync(payment.TotalAmount, payment.Booking.LocationId);

                            // Create fee distribution transactions
                            await _transactionService.CreatePaymentDistributionTransactionsAsync(payment.PaymentId, calculationResult.PlatformFee, calculationResult.PhotographerPayout, calculationResult.LocationFee);

                            // Update booking status
                            payment.Booking.Status = "Confirmed";
                            payment.Booking.UpdatedAt = DateTime.UtcNow;

                            // Update photographer wallet
                            if (payment.Booking.PhotographerId != null && calculationResult.PhotographerPayout > 0)
                            {
                                var photographer = await _context.Photographers
                                    .FirstOrDefaultAsync(p => p.PhotographerId == payment.Booking.PhotographerId);
                                
                                if (photographer != null)
                                {
                                    await _walletService.AddFundsToWalletAsync(photographer.UserId, calculationResult.PhotographerPayout);
                                }
                            }

                            // Update location owner wallet (only for registered locations)
                            if (payment.Booking.LocationId != null && calculationResult.LocationFee > 0)
                            {
                                var locationWithOwner = await _context.Locations
                                    .Include(l => l.LocationOwner)
                                    .FirstOrDefaultAsync(l => l.LocationId == payment.Booking.LocationId);
                                
                                // Only pay location owner if it's a registered location
                                if (locationWithOwner?.LocationOwner != null && 
                                    (locationWithOwner.LocationType == "Registered" || locationWithOwner.LocationType == null))
                                {
                                    await _walletService.AddFundsToWalletAsync(locationWithOwner.LocationOwner.UserId, calculationResult.LocationFee);
                                }
                            }
                        }
                        else
                        {
                            // This is a wallet top-up - just add funds to user wallet
                            await _walletService.AddFundsToWalletAsync(payment.CustomerId, payment.TotalAmount);
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
        */

        public async Task<PaymentResponse> GetPaymentStatusAsync(long paymentId)
        {
            try
            {
                // Find the payment record in our database only
                var payment = await _context.Payments
                    .Include(p => p.Booking)
                    .ThenInclude(b => b.Photographer)
                        .ThenInclude(p => p.User)
                    .Include(p => p.Booking)
                        .ThenInclude(b => b.Location)
                        .ThenInclude(l => l.LocationOwner)
                        .ThenInclude(lo => lo.User)
                    .Include(p => p.Customer)
                    .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

                if (payment == null)
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Payment not found",
                        Data = null
                    };
                }

                // Create a comprehensive payment status response from database only
                var paymentStatusData = new
                {
                    
                    PaymentId = payment.PaymentId,
                    ExternalTransactionId = payment.ExternalTransactionId,
                    CustomerId = payment.CustomerId,
                    CustomerName = payment.Customer?.FullName,
                    CustomerEmail = payment.Customer?.Email,
                    TotalAmount = payment.TotalAmount,
                    status = payment.Status.ToString(),
                    Currency = payment.Currency,
                    Method = payment.Method,
                    Note = payment.Note,
                    CreatedAt = payment.CreatedAt,
                    UpdatedAt = payment.UpdatedAt,
                    BookingId = payment.BookingId,
                    BookingStatus = payment.Booking?.Status,
                    PhotographerName = payment.Booking?.Photographer?.User?.FullName,
                    LocationName = payment.Booking?.Location?.Name,
                    IsWalletTopUp = !payment.BookingId.HasValue
                };

                return new PaymentResponse
                {
                    Error = 0,
                    Message = "Payment status retrieved successfully",
                    Data = paymentStatusData
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
                    .FirstOrDefaultAsync(p => p.BookingId == bookingId && p.BookingId.HasValue);
            
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

                // Only add funds to user wallet (do not update booking or distribute funds)
                await _walletService.AddFundsToWalletAsync(payment.CustomerId, payment.TotalAmount);

                await _unitOfWork.SaveChangesAsync();
                var WallTransaction = new SnapLink_Repository.Entity.Transaction
                {
                    ReferencePaymentId = payment.PaymentId,
                    FromUserId = payment.CustomerId,
                    ToUserId = null, // System holds the funds
                    Amount = payment.TotalAmount,
                    Type = TransactionType.Deposit,          
                    Status = TransactionStatus.Success,
                    Note = $"Top up to Wallet ",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(WallTransaction);
                await _unitOfWork.SaveChangesAsync();

                // Send push notification about successful payment
                try
                {
                    await _pushNotificationService.SendPaymentNotificationAsync(
                        payment.CustomerId,
                        "success",
                        payment.TotalAmount,
                        payment.PaymentId
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send payment success notification: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Payment {payment.PaymentId} already has Success status");
            }
        }
    }
}