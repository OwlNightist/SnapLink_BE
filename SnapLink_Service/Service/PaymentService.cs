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
using Microsoft.Extensions.Options;
using SnapLink_Model.Configuration;

namespace SnapLink_Service.Service
{
public class PaymentService : IPaymentService
{
    private readonly PayOS _payOS;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SnaplinkDbContext _context;
        private readonly IWalletService _walletService;
        private readonly PaymentSettings _paymentSettings;

        public PaymentService(PayOS payOS, IUnitOfWork unitOfWork, SnaplinkDbContext context, IWalletService walletService, IOptions<PaymentSettings> paymentSettings)
    {
        _payOS = payOS;
        _unitOfWork = unitOfWork;
        _context = context;
            _walletService = walletService;
            _paymentSettings = paymentSettings.Value;
    }
// need userid for now , add to jwt token later
    public async Task<PaymentResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request, int userId)
    {
        try
        {
                // Validate photographer exists
                var photographer = await _context.Photographers
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.PhotographerId == request.PhotographerId);
                
                if (photographer == null)
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Photographer not found",
                        Data = null
                    };
                }

                // Validate location exists
                var location = await _context.Locations
                    .Include(l => l.LocationOwner)
                    .ThenInclude(lo => lo.User)
                    .FirstOrDefaultAsync(l => l.LocationId == request.LocationId);
                
                if (location == null)
                {
                    return new PaymentResponse
                    {
                        Error = -1,
                        Message = "Location not found",
                        Data = null
                    };
                }

                // Calculate payment distribution based on location type and fees
                decimal totalAmount = request.Price;
                decimal platformFee = totalAmount * (_paymentSettings.PlatformFeePercentage / 100m);
                decimal locationFee = 0m;
                decimal photographerPayout;

                // Determine location fee based on location type
                if (location.LocationType == "Registered" || location.LocationType == null) // Default to registered for backward compatibility
                {
                    locationFee = location.HourlyRate ?? 0; // Fixed location fee for registered locations
                }
                // External locations (Google Places) have no fee

                // Calculate photographer payout
                if (locationFee > 0)
                {
                    // Registered location with fee - photographer gets remaining after platform fee and location fee
                    photographerPayout = totalAmount - platformFee - locationFee;
                }
                else
                {
                    // External location or no location fee - photographer gets remaining after platform fee
                    photographerPayout = totalAmount - platformFee;
                }

                // Generate unique payment code
                int paymentCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                
                // Create PayOS item data
                ItemData item = new ItemData(request.ProductName, 1, (int)totalAmount);
            List<ItemData> items = new List<ItemData> { item };
            
                // Create payment data
            PaymentData paymentData = new PaymentData(
                    paymentCode, 
                    (int)totalAmount, 
                request.Description, 
                items, 
                request.CancelUrl, 
                request.ReturnUrl
            );

            // Create payment link with PayOS
            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

                // Create transaction record
            var dbTransaction = new SnapLink_Repository.Entity.Transaction
            {
                UserId = userId,
                    Amount = totalAmount,
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
                    TotalPrice = totalAmount,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // Create payment record
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                    Amount = totalAmount,
                PaymentMethod = "PayOS",
                Status = "Pending",
                    TransactionId = paymentCode.ToString(),
                    PhotographerPayoutAmount = photographerPayout,
                    LocationOwnerPayoutAmount = locationFee,
                    PlatformFee = platformFee,
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
                        .FirstOrDefaultAsync(p => p.TransactionId == paymentId.ToString());
                
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
                        if (payment.Booking?.PhotographerId != null && payment.PhotographerPayoutAmount > 0)
                        {
                            var photographer = await _context.Photographers
                                .FirstOrDefaultAsync(p => p.PhotographerId == payment.Booking.PhotographerId);
                            
                            if (photographer != null)
                            {
                                await _walletService.AddFundsToWalletAsync(photographer.UserId, payment.PhotographerPayoutAmount.Value);
                            }
                        }

                        // Update location owner wallet (only for registered locations)
                        if (payment.Booking?.LocationId != null && payment.LocationOwnerPayoutAmount > 0)
                        {
                            var location = await _context.Locations
                                .Include(l => l.LocationOwner)
                                .FirstOrDefaultAsync(l => l.LocationId == payment.Booking.LocationId);
                            
                            // Only pay location owner if it's a registered location
                            if (location?.LocationOwner != null && 
                                (location.LocationType == "Registered" || location.LocationType == null))
                            {
                                await _walletService.AddFundsToWalletAsync(location.LocationOwner.UserId, payment.LocationOwnerPayoutAmount.Value);
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
                    .FirstOrDefaultAsync(p => p.TransactionId == paymentId.ToString());
            
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
    }
} 