using Microsoft.EntityFrameworkCore;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SnaplinkDbContext _context;

        public NotificationRepository(SnaplinkDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetAllAsync() =>
            await _context.Notifications.Include(n => n.User).ToListAsync();

        public async Task<Notification?> GetByIdAsync(int id) =>
            await _context.Notifications.Include(n => n.User).FirstOrDefaultAsync(n => n.MotificationId == id);

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId) =>
            await _context.Notifications.Include(n => n.User)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task AddAsync(Notification notification) =>
            await _context.Notifications.AddAsync(notification);

        public Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Notification notification)
        {
            _context.Notifications.Remove(notification);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
