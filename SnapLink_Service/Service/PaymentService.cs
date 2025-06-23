using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service;

public class PaymentService : IPaymentService
{
    private readonly PayOS _payOS;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SnaplinkDbContext _context;

    public PaymentService(PayOS payOS, IUnitOfWork unitOfWork, SnaplinkDbContext context)
    {
        _payOS = payOS;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<PaymentResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request, int userId)
    {
        try
        {
            // Generate unique order code
            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
            
            // Create PayOS item data - convert decimal to int (PayOS expects int for amount)
            ItemData item = new ItemData(request.ProductName, 1, (int)request.Price);
            List<ItemData> items = new List<ItemData> { item };
            
            // Create payment data - convert decimal to int
            PaymentData paymentData = new PaymentData(
                orderCode, 
                (int)request.Price, 
                request.Description, 
                items, 
                request.CancelUrl, 
                request.ReturnUrl
            );

            // Create payment link with PayOS
            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

            // Create transaction record - use fully qualified name to avoid ambiguity
            var dbTransaction = new SnapLink_Repository.Entity.Transaction
            {
                UserId = userId,
                Amount = request.Price,
                Type = "Payment",
                Description = request.Description,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TransactionRepository.AddAsync(dbTransaction);
            await _unitOfWork.SaveChangesAsync();

            // Create a booking record for this payment
            var booking = new Booking
            {
                UserId = userId,
                PhotographerId = request.PhotographerId,
                LocationId = request.LocationId,
                Status = "Pending",
                TotalPrice = request.Price,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // Create payment record
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = request.Price,
                PaymentMethod = "PayOS",
                Status = "Pending",
                TransactionId = orderCode.ToString(),
                PhotographerPayoutAmount = request.Price,
                //PhotographerPayoutAmount = request.Price * 0.9m, // 90% to photographer
                //PlatformFee = request.Price * 0.1m, // 10% platform fee
                CreatedAt = DateTime.UtcNow
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

    public async Task<PaymentResponse> GetOrderAsync(int orderId)
    {
        try
        {
            // Get payment information from PayOS
            PaymentLinkInformation paymentLinkInformation = await _payOS.getPaymentLinkInformation(orderId);
            
            if (paymentLinkInformation.status.Equals("PAID"))
            {
                // Find the payment record
                var payment = await _context.Payments
                    .Include(p => p.Booking)
                    .ThenInclude(b => b.Photographer)
                    .FirstOrDefaultAsync(p => p.TransactionId == orderId.ToString());
                
                if (payment != null && payment.Status != "Completed")
                {
                    // Update payment status
                    payment.Status = "Completed";
                    payment.UpdatedAt = DateTime.UtcNow;
                    
                    // Update transaction status
                    var transaction = await _context.Transactions
                        .FirstOrDefaultAsync(t => t.TransactionId.ToString() == payment.TransactionId);
                    if (transaction != null)
                    {
                        transaction.Status = "Completed";
                    }

                    // Update booking status
                    if (payment.Booking != null)
                    {
                        payment.Booking.Status = "Confirmed";
                        payment.Booking.UpdatedAt = DateTime.UtcNow;
                    }

                    // Update photographer wallet
                    if (payment.Booking?.PhotographerId != null)
                    {
                        var photographerWallet = await _context.PhotographerWallets
                            .FirstOrDefaultAsync(w => w.PhotographerId == payment.Booking.PhotographerId);
                        
                        if (photographerWallet != null)
                        {
                            photographerWallet.Balance = (photographerWallet.Balance ?? 0) + (payment.PhotographerPayoutAmount ?? 0);
                            photographerWallet.UpdatedAt = DateTime.UtcNow;
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                }
            }

            return new PaymentResponse
            {
                Error = 0,
                Message = "Order retrieved successfully",
                Data = paymentLinkInformation
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting order: {ex.Message}");
            return new PaymentResponse
            {
                Error = -1,
                Message = "Failed to get order",
                Data = null
            };
        }
    }

    public async Task<PaymentResponse> CancelOrderAsync(int orderId)
    {
        try
        {
            PaymentLinkInformation paymentLinkInformation = await _payOS.cancelPaymentLink(orderId);
            
            // Update payment status to cancelled
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == orderId.ToString());
            
            if (payment != null)
            {
                payment.Status = "Cancelled";
                payment.UpdatedAt = DateTime.UtcNow;
                
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
                Message = "Order cancelled successfully",
                Data = paymentLinkInformation
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cancelling order: {ex.Message}");
            return new PaymentResponse
            {
                Error = -1,
                Message = "Failed to cancel order",
                Data = null
            };
        }
    }

    public async Task<PaymentResponse> ConfirmWebhookAsync(ConfirmWebhookRequest request)
    {
        try
        {
            await _payOS.confirmWebhook(request.WebhookUrl);
            return new PaymentResponse
            {
                Error = 0,
                Message = "Webhook confirmed successfully",
                Data = null
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error confirming webhook: {ex.Message}");
            return new PaymentResponse
            {
                Error = -1,
                Message = "Failed to confirm webhook",
                Data = null
            };
        }
    }
} 