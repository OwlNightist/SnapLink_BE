using Microsoft.EntityFrameworkCore;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnapLink_Repository.Repository
{
    public class PhotoDeliveryRepository : IPhotoDeliveryRepository
    {
        private readonly SnaplinkDbContext _context;

        public PhotoDeliveryRepository(SnaplinkDbContext context)
        {
            _context = context;
        }

        public async Task<PhotoDelivery?> GetPhotoDeliveryByBookingIdAsync(int bookingId)
        {
            return await _context.PhotoDeliveries
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Photographer)
                .ThenInclude(p => p.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Location)
                .FirstOrDefaultAsync(pd => pd.BookingId == bookingId);
        }

        public async Task<PhotoDelivery?> GetPhotoDeliveryByIdAsync(int photoDeliveryId)
        {
            return await _context.PhotoDeliveries
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Photographer)
                .ThenInclude(p => p.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Location)
                .FirstOrDefaultAsync(pd => pd.PhotoDeliveryId == photoDeliveryId);
        }

        public async Task<IEnumerable<PhotoDelivery>> GetPhotoDeliveriesByPhotographerIdAsync(int photographerId)
        {
            return await _context.PhotoDeliveries
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Location)
                .Where(pd => pd.Booking.PhotographerId == photographerId)
                .OrderByDescending(pd => pd.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhotoDelivery>> GetPhotoDeliveriesByCustomerIdAsync(int customerId)
        {
            return await _context.PhotoDeliveries
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Photographer)
                .ThenInclude(p => p.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Location)
                .Where(pd => pd.Booking.UserId == customerId)
                .OrderByDescending(pd => pd.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhotoDelivery>> GetPhotoDeliveriesByStatusAsync(string status)
        {
            return await _context.PhotoDeliveries
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Photographer)
                .ThenInclude(p => p.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Location)
                .Where(pd => pd.Status == status)
                .OrderByDescending(pd => pd.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhotoDelivery>> GetPendingPhotoDeliveriesAsync()
        {
            return await _context.PhotoDeliveries
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Photographer)
                .ThenInclude(p => p.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Location)
                .Where(pd => pd.Status == "Pending" && pd.DeliveryMethod == "PhotographerDevice")
                .OrderBy(pd => pd.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhotoDelivery>> GetExpiredPhotoDeliveriesAsync()
        {
            return await _context.PhotoDeliveries
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Photographer)
                .ThenInclude(p => p.User)
                .Include(pd => pd.Booking)
                .ThenInclude(b => b.Location)
                .Where(pd => pd.ExpiresAt.HasValue && pd.ExpiresAt < DateTime.UtcNow)
                .OrderByDescending(pd => pd.ExpiresAt)
                .ToListAsync();
        }

        public async Task AddPhotoDeliveryAsync(PhotoDelivery photoDelivery)
        {
            photoDelivery.CreatedAt = DateTime.UtcNow;
            await _context.PhotoDeliveries.AddAsync(photoDelivery);
        }

        public async Task UpdatePhotoDeliveryAsync(PhotoDelivery photoDelivery)
        {
            photoDelivery.UpdatedAt = DateTime.UtcNow;
            _context.PhotoDeliveries.Update(photoDelivery);
        }

        public async Task DeletePhotoDeliveryAsync(PhotoDelivery photoDelivery)
        {
            _context.PhotoDeliveries.Remove(photoDelivery);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 