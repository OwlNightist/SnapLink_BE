using SnapLink_Model.DTO;
using SnapLink_Repository.Entity;
using SnapLink_Repository.IRepository;
using SnapLink_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;

        public NotificationService(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Notification>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Notification?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task CreateAsync(NotificationDto dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Content = dto.Content,
                NotificationType = dto.NotificationType,
                ReferenceId = dto.ReferenceId,
                ReadStatus = dto.ReadStatus ?? false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(notification);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, NotificationDto dto)
        {
            var notification = await _repo.GetByIdAsync(id);
            if (notification == null) throw new Exception("Notification not found");

            notification.Title = dto.Title;
            notification.Content = dto.Content;
            notification.NotificationType = dto.NotificationType;
            notification.ReferenceId = dto.ReferenceId;
            notification.ReadStatus = dto.ReadStatus ?? notification.ReadStatus;

            await _repo.UpdateAsync(notification);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var notification = await _repo.GetByIdAsync(id);
            if (notification == null) throw new Exception("Notification not found");

            await _repo.DeleteAsync(notification);
            await _repo.SaveChangesAsync();
        }
    }
}
