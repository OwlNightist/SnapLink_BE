using Microsoft.EntityFrameworkCore;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class BookingService : IBookingService
    {
        private readonly SnaplinkDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAvailabilityService _availabilityService;
        private readonly IWalletService _walletService;
        private readonly IPaymentCalculationService _paymentCalculationService;

        public BookingService(SnaplinkDbContext context, IUnitOfWork unitOfWork, IAvailabilityService availabilityService, IWalletService walletService, IPaymentCalculationService paymentCalculationService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _availabilityService = availabilityService;
            _walletService = walletService;
            _paymentCalculationService = paymentCalculationService;
        }

        public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request, int userId)
        {
            try
            {
                // Validate photographer exists
                var photographer = await _context.Photographers
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.PhotographerId == request.PhotographerId);
                
                if (photographer == null)
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Photographer not found",
                        Data = null
                    };
                }

                // Validate user exists
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId);
                
                if (user == null)
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "User not found",
                        Data = null
                    };
                }

                // Handle location validation and creation
                int? locationId = request.LocationId;
                
                if (locationId.HasValue)
                {
                    // Registered location - validate it exists
                    var location = await _context.Locations
                        .FirstOrDefaultAsync(l => l.LocationId == locationId.Value);
                    
                    if (location == null)
                    {
                        return new BookingResponse
                        {
                            Error = -1,
                            Message = "Location not found",
                            Data = null
                        };
                    }

                    // Check location availability for this specific photographer
                    if (!await IsLocationAvailableForPhotographerAsync(locationId.Value, request.PhotographerId, request.StartDatetime, request.EndDatetime))
                    {
                        return new BookingResponse
                        {
                            Error = -1,
                            Message = "Photographer already has a booking at this location during the selected time",
                            Data = null
                        };
                    }
                }
                else
                {
                    // External location - validate external location data
                    if (request.ExternalLocation == null)
                    {
                        return new BookingResponse
                        {
                            Error = -1,
                            Message = "External location details are required when LocationId is not provided",
                            Data = null
                        };
                    }

                    // Create external location record
                    var externalLocation = new Location
                    {
                        LocationType = "External",
                        ExternalPlaceId = request.ExternalLocation.PlaceId,
                        Name = request.ExternalLocation.Name,
                        Address = request.ExternalLocation.Address,
                        Description = request.ExternalLocation.Description,
                        HourlyRate = 0, // External locations have no fee
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.LocationRepository.AddAsync(externalLocation);
                    await _unitOfWork.SaveChangesAsync();
                    
                    locationId = externalLocation.LocationId;
                }

                // Check photographer availability
                if (!await IsPhotographerAvailableAsync(request.PhotographerId, request.StartDatetime, request.EndDatetime))
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Photographer is not available for the selected time",
                        Data = null
                    };
                }

                // Calculate true price automatically
                var truePrice = await CalculateBookingPriceAsync(request.PhotographerId, locationId, request.StartDatetime, request.EndDatetime);
                
                // Get display price (capped for testing)
                var displayPrice = GetDisplayPrice(truePrice);

                // Create booking with true price stored in database
                var booking = new Booking
                {
                    UserId = userId,
                    PhotographerId = request.PhotographerId,
                    LocationId = locationId.Value,
                    StartDatetime = request.StartDatetime,
                    EndDatetime = request.EndDatetime,
                    Status = "Pending", // Requires payment to be confirmed
                    SpecialRequests = request.SpecialRequests,
                    TotalPrice = truePrice, // Store true price in database
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.BookingRepository.AddAsync(booking);
                await _unitOfWork.SaveChangesAsync();

                // Return booking data with display price
                var bookingData = await MapBookingToResponseAsync(booking, displayPrice);
                
                return new BookingResponse
                {
                    Error = 0,
                    Message = "Booking created successfully. Payment required to confirm booking.",
                    Data = bookingData
                };
            }
            catch (Exception ex)
            {
                return new BookingResponse
                {
                    Error = -1,
                    Message = $"Failed to create booking: {ex.Message}",
                    Data = null
                };
            }
        }



        public async Task<BookingResponse> GetBookingByIdAsync(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Photographer)
                    .ThenInclude(p => p.User)
                    .Include(b => b.Location)
                    .Include(b => b.Payment)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Booking not found",
                        Data = null
                    };
                }

                var bookingData = await MapBookingToResponseAsync(booking);
                
                return new BookingResponse
                {
                    Error = 0,
                    Message = "Booking retrieved successfully",
                    Data = bookingData
                };
            }
            catch (Exception ex)
            {
                return new BookingResponse
                {
                    Error = -1,
                    Message = $"Failed to get booking: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<BookingListResponse> GetUserBookingsAsync(int userId, int page = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Photographer)
                    .ThenInclude(p => p.User)
                    .Include(b => b.Location)
                    .Include(b => b.Payment)
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.CreatedAt);

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var bookings = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var bookingDataList = new List<BookingData>();
                foreach (var booking in bookings)
                {
                    bookingDataList.Add(await MapBookingToResponseAsync(booking));
                }

                return new BookingListResponse
                {
                    Error = 0,
                    Message = "User bookings retrieved successfully",
                    Data = new BookingListData
                    {
                        Bookings = bookingDataList,
                        TotalCount = totalCount,
                        Page = page,
                        PageSize = pageSize,
                        TotalPages = totalPages
                    }
                };
            }
            catch (Exception ex)
            {
                return new BookingListResponse
                {
                    Error = -1,
                    Message = $"Failed to get user bookings: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<BookingListResponse> GetPhotographerBookingsAsync(int photographerId, int page = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Photographer)
                    .ThenInclude(p => p.User)
                    .Include(b => b.Location)
                    .Include(b => b.Payment)
                    .Where(b => b.PhotographerId == photographerId)
                    .OrderByDescending(b => b.CreatedAt);

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var bookings = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var bookingDataList = new List<BookingData>();
                foreach (var booking in bookings)
                {
                    bookingDataList.Add(await MapBookingToResponseAsync(booking));
                }

                return new BookingListResponse
                {
                    Error = 0,
                    Message = "Photographer bookings retrieved successfully",
                    Data = new BookingListData
                    {
                        Bookings = bookingDataList,
                        TotalCount = totalCount,
                        Page = page,
                        PageSize = pageSize,
                        TotalPages = totalPages
                    }
                };
            }
            catch (Exception ex)
            {
                return new BookingListResponse
                {
                    Error = -1,
                    Message = $"Failed to get photographer bookings: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<BookingResponse> UpdateBookingAsync(int bookingId, UpdateBookingRequest request)
        {
            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Booking not found",
                        Data = null
                    };
                }

                // Only allow updates if booking is still pending
                if (booking.Status != "Pending")
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Cannot update booking that is not in pending status",
                        Data = null
                    };
                }

                // Update fields
                if (request.StartDatetime.HasValue)
                    booking.StartDatetime = request.StartDatetime.Value;
                
                if (request.EndDatetime.HasValue)
                    booking.EndDatetime = request.EndDatetime.Value;
                
                if (request.SpecialRequests != null)
                    booking.SpecialRequests = request.SpecialRequests;
                
                if (request.Status != null)
                    booking.Status = request.Status;

                booking.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                var bookingData = await MapBookingToResponseAsync(booking);
                
                return new BookingResponse
                {
                    Error = 0,
                    Message = "Booking updated successfully",
                    Data = bookingData
                };
            }
            catch (Exception ex)
            {
                return new BookingResponse
                {
                    Error = -1,
                    Message = $"Failed to update booking: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<BookingResponse> CancelBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Booking not found",
                        Data = null
                    };
                }

                // Only allow cancellation if booking is pending or confirmed
                if (booking.Status != "Pending" && booking.Status != "Confirmed")
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Cannot cancel booking in current status",
                        Data = null
                    };
                }

                booking.Status = "Cancelled";
                booking.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                var bookingData = await MapBookingToResponseAsync(booking);
                
                return new BookingResponse
                {
                    Error = 0,
                    Message = "Booking cancelled successfully",
                    Data = bookingData
                };
            }
            catch (Exception ex)
            {
                return new BookingResponse
                {
                    Error = -1,
                    Message = $"Failed to cancel booking: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<BookingResponse> CompleteBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Booking not found",
                        Data = null
                    };
                }

                // Only allow completion if booking is confirmed
                if (booking.Status != "Confirmed")
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Cannot complete booking that is not confirmed",
                        Data = null
                    };
                }

                booking.Status = "Completed";
                booking.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                var bookingData = await MapBookingToResponseAsync(booking);
                
                return new BookingResponse
                {
                    Error = 0,
                    Message = "Booking completed successfully",
                    Data = bookingData
                };
            }
            catch (Exception ex)
            {
                return new BookingResponse
                {
                    Error = -1,
                    Message = $"Failed to complete booking: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<BookingResponse> ConfirmBookingAsync(int bookingId, int userId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Photographer)
                    .Include(b => b.Location)
                        .ThenInclude(l => l.LocationOwner)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId);
                if (booking == null)
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Booking not found or does not belong to user",
                        Data = null
                    };
                }
                if (booking.Status != "Pending")
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Booking is not in pending status",
                        Data = null
                    };
                }
                if (booking.TotalPrice == null || booking.TotalPrice <= 0)
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Invalid booking price",
                        Data = null
                    };
                }
                // Check wallet balance
                var balance = await _walletService.GetWalletBalanceAsync(userId);
                if (balance < booking.TotalPrice.Value)
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Insufficient wallet balance",
                        Data = null
                    };
                }
                // Deduct funds
                var deducted = await _walletService.DeductFundsFromWalletAsync(userId, booking.TotalPrice.Value);
                if (!deducted)
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = "Failed to deduct funds from wallet",
                        Data = null
                    };
                }
                // Distribute funds
                var calculationResult = await _paymentCalculationService.CalculatePaymentDistributionAsync(booking.TotalPrice.Value, booking.Location);
                // Photographer payout
                if (calculationResult.PhotographerPayout > 0 && booking.Photographer != null)
                {
                    await _walletService.AddFundsToWalletAsync(booking.Photographer.UserId, calculationResult.PhotographerPayout);
                }
                // Location owner payout (only for registered locations)
                if (calculationResult.LocationFee > 0 && booking.Location?.LocationOwner != null && (booking.Location.LocationType == "Registered" || booking.Location.LocationType == null))
                {
                    await _walletService.AddFundsToWalletAsync(booking.Location.LocationOwner.UserId, calculationResult.LocationFee);
                }
                // Update booking status
                booking.Status = "Confirmed";
                booking.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
                var bookingData = await MapBookingToResponseAsync(booking);
                return new BookingResponse
                {
                    Error = 0,
                    Message = "Booking confirmed and funds distributed successfully",
                    Data = bookingData
                };
            }
            catch (Exception ex)
            {
                return new BookingResponse
                {
                    Error = -1,
                    Message = $"Failed to confirm booking: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<bool> IsPhotographerAvailableAsync(int photographerId, DateTime startTime, DateTime endTime)
        {
            // Check if photographer has any conflicting bookings
            var conflictingBookings = await _context.Bookings
                .Where(b => b.PhotographerId == photographerId &&
                           b.Status != "Cancelled" &&
                           b.Status != "Completed" &&
                           ((b.StartDatetime <= startTime && b.EndDatetime > startTime) ||
                            (b.StartDatetime < endTime && b.EndDatetime >= endTime) ||
                            (b.StartDatetime >= startTime && b.EndDatetime <= endTime)))
                .AnyAsync();

            if (conflictingBookings)
                return false;

            // Use AvailabilityService to check photographer's availability schedule
            return await _availabilityService.IsPhotographerAvailableAsync(photographerId, startTime, endTime);
        }

        public async Task<bool> IsLocationAvailableAsync(int locationId, DateTime startTime, DateTime endTime)
        {
            // Check if location has any conflicting bookings for the same time slot
            var conflictingBookings = await _context.Bookings
                .Where(b => b.LocationId == locationId &&
                           b.Status != "Cancelled" &&
                           b.Status != "Completed" &&
                           ((b.StartDatetime <= startTime && b.EndDatetime > startTime) ||
                            (b.StartDatetime < endTime && b.EndDatetime >= endTime) ||
                            (b.StartDatetime >= startTime && b.EndDatetime <= endTime)))
                .AnyAsync();

            return !conflictingBookings;
        }

        /// <summary>
        /// Check if location is available for a specific photographer and time
        /// This allows multiple photographers to book the same location at the same time
        /// </summary>
        public async Task<bool> IsLocationAvailableForPhotographerAsync(int locationId, int photographerId, DateTime startTime, DateTime endTime)
        {
            // Check if this specific photographer already has a booking for this location at the same time
            var photographerConflict = await _context.Bookings
                .Where(b => b.LocationId == locationId &&
                           b.PhotographerId == photographerId &&
                           b.Status != "Cancelled" &&
                           b.Status != "Completed" &&
                           ((b.StartDatetime <= startTime && b.EndDatetime > startTime) ||
                            (b.StartDatetime < endTime && b.EndDatetime >= endTime) ||
                            (b.StartDatetime >= startTime && b.EndDatetime <= endTime)))
                .AnyAsync();

            // Only check if this specific photographer has conflict
            // Multiple photographers can book the same location at the same time
            return !photographerConflict;
        }

        /// <summary>
        /// Get all photographers booking the same location at the same time
        /// </summary>
        public async Task<IEnumerable<BookingData>> GetPhotographersAtLocationAsync(int locationId, DateTime startTime, DateTime endTime)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Photographer)
                .ThenInclude(p => p.User)
                .Include(b => b.User)
                .Where(b => b.LocationId == locationId &&
                           b.Status != "Cancelled" &&
                           b.Status != "Completed" &&
                           ((b.StartDatetime <= startTime && b.EndDatetime > startTime) ||
                            (b.StartDatetime < endTime && b.EndDatetime >= endTime) ||
                            (b.StartDatetime >= startTime && b.EndDatetime <= endTime)))
                .ToListAsync();

            var bookingDataList = new List<BookingData>();
            foreach (var booking in bookings)
            {
                var bookingData = await MapBookingToResponseAsync(booking);
                bookingDataList.Add(bookingData);
            }

            return bookingDataList;
        }

        public async Task<decimal> CalculateBookingPriceAsync(int photographerId, int? locationId, DateTime startTime, DateTime endTime)
        {
            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

            if (photographer == null)
                return 0;

            var duration = (endTime - startTime).TotalHours;
            var photographerHourlyRate = photographer.HourlyRate ?? 0;
            
            // If no location ID provided, it's an external location (no fee)
            if (!locationId.HasValue)
            {
                return photographerHourlyRate * (decimal)duration;
            }

            // Check if location exists and determine fee
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.LocationId == locationId.Value);

            if (location == null)
                return photographerHourlyRate * (decimal)duration; // Default to photographer only

            // Only add location fee if it's a registered location
            var locationHourlyRate = 0m;
            if (location.LocationType == "Registered" || location.LocationType == null) // Default to registered for backward compatibility
            {
                locationHourlyRate = location.HourlyRate ?? 0;
            }
            // External locations (Google Places) have no fee

            return (photographerHourlyRate + locationHourlyRate) * (decimal)duration;
        }

        public async Task<int> CancelAllPendingBookingsAsync()
        {
            try
            {
                // Find all pending bookings (regardless of age)
                var pendingBookings = await _context.Bookings
                    .Where(b => b.Status == "Pending")
                    .ToListAsync();

                int cancelledCount = 0;
                foreach (var booking in pendingBookings)
                {
                    booking.Status = "Cancelled";
                    booking.UpdatedAt = DateTime.UtcNow;
                    cancelledCount++;
                    
                    Console.WriteLine($"Cancelled pending booking {booking.BookingId} created at {booking.CreatedAt}");
                }

                if (cancelledCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                    Console.WriteLine($"Cancelled {cancelledCount} pending bookings");
                }

                return cancelledCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling pending bookings: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> CancelExpiredPendingBookingsAsync()
        {
            try
            {
                // Get timeout duration (default 3 minutes for testing)
                var timeoutMinutes = 3; // Can be moved to configuration later
                var timeoutThreshold = DateTime.UtcNow.AddMinutes(-timeoutMinutes);

                // Find all pending bookings that are older than the timeout threshold
                var expiredBookings = await _context.Bookings
                    .Where(b => b.Status == "Pending" && b.CreatedAt < timeoutThreshold)
                    .ToListAsync();

                int cancelledCount = 0;
                foreach (var booking in expiredBookings)
                {
                    booking.Status = "Cancelled";
                    booking.UpdatedAt = DateTime.UtcNow;
                    cancelledCount++;
                    
                    Console.WriteLine($"Cancelled expired pending booking {booking.BookingId} created at {booking.CreatedAt}");
                }

                if (cancelledCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                    Console.WriteLine($"Cancelled {cancelledCount} expired pending bookings");
                }

                return cancelledCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling expired bookings: {ex.Message}");
                return 0;
            }
        }

        private decimal GetDisplayPrice(decimal truePrice)
        {
            // For testing: hard-code display price at 5000, but keep true price in database
            return 5000m;
        }

        private async Task<BookingData> MapBookingToResponseAsync(Booking booking, decimal? displayPrice = null)
        {
            var duration = (booking.EndDatetime - booking.StartDatetime)?.TotalHours ?? 0;
            var truePrice = booking.TotalPrice ?? 0;
            var priceToDisplay = displayPrice ?? GetDisplayPrice(truePrice);
            var pricePerHour = duration > 0 ? priceToDisplay / (decimal)duration : 0;

            return new BookingData
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                UserName = booking.User?.FullName ?? "",
                UserEmail = booking.User?.Email ?? "",
                PhotographerId = booking.PhotographerId,
                PhotographerName = booking.Photographer?.User?.FullName ?? "",
                PhotographerEmail = booking.Photographer?.User?.Email ?? "",
                LocationId = booking.LocationId,
                LocationName = booking.Location?.Name ?? "",
                LocationAddress = booking.Location?.Address ?? "",
                StartDatetime = booking.StartDatetime ?? DateTime.UtcNow,
                EndDatetime = booking.EndDatetime ?? DateTime.UtcNow,
                Status = booking.Status ?? "",
                SpecialRequests = booking.SpecialRequests,
                TotalPrice = priceToDisplay, // Use display price for response
                CreatedAt = booking.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = booking.UpdatedAt,
                HasPayment = booking.Payment != null,
                PaymentStatus = booking.Payment?.Status.ToString() ?? "",
                PaymentAmount = booking.Payment?.TotalAmount,
                DurationHours = (int)duration,
                PricePerHour = pricePerHour
            };
        }
    }
} 