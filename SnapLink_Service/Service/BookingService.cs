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

        public BookingService(SnaplinkDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
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

                    // Check location availability
                    if (!await IsLocationAvailableAsync(locationId.Value, request.StartDatetime, request.EndDatetime))
                    {
                        return new BookingResponse
                        {
                            Error = -1,
                            Message = "Location is not available for the selected time",
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

                // Calculate price
                var calculatedPrice = await CalculateBookingPriceAsync(request.PhotographerId, locationId, request.StartDatetime, request.EndDatetime);
                
                if (Math.Abs(calculatedPrice - request.TotalPrice) > 0.01m)
                {
                    return new BookingResponse
                    {
                        Error = -1,
                        Message = $"Price mismatch. Calculated price: {calculatedPrice:C}, provided price: {request.TotalPrice:C}",
                        Data = null
                    };
                }

                // Create booking
                var booking = new Booking
                {
                    UserId = userId,
                    PhotographerId = request.PhotographerId,
                    LocationId = locationId.Value,
                    StartDatetime = request.StartDatetime,
                    EndDatetime = request.EndDatetime,
                    Status = "Pending", // Requires payment to be confirmed
                    SpecialRequests = request.SpecialRequests,
                    TotalPrice = request.TotalPrice,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.BookingRepository.AddAsync(booking);
                await _unitOfWork.SaveChangesAsync();

                // Return booking data
                var bookingData = await MapBookingToResponseAsync(booking);
                
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
                
                if (request.TotalPrice.HasValue)
                    booking.TotalPrice = request.TotalPrice.Value;
                
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

            return !conflictingBookings;
        }

        public async Task<bool> IsLocationAvailableAsync(int locationId, DateTime startTime, DateTime endTime)
        {
            // Check if location has any conflicting bookings
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

        private async Task<BookingData> MapBookingToResponseAsync(Booking booking)
        {
            var duration = (booking.EndDatetime - booking.StartDatetime)?.TotalHours ?? 0;
            var pricePerHour = duration > 0 ? (booking.TotalPrice ?? 0) / (decimal)duration : 0;

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
                TotalPrice = booking.TotalPrice ?? 0,
                CreatedAt = booking.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = booking.UpdatedAt,
                HasPayment = booking.Payment != null,
                PaymentStatus = booking.Payment?.Status ?? "",
                PaymentAmount = booking.Payment?.Amount,
                DurationHours = (int)duration,
                PricePerHour = pricePerHour
            };
        }
    }
} 