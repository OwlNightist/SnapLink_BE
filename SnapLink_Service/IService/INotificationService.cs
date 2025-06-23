using SnapLink_Model.DTO;
using SnapLink_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Service.IService
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetAllAsync();
        Task<Notification?> GetByIdAsync(int id);
        Task CreateAsync(NotificationDto dto);
        Task UpdateAsync(int id, NotificationDto dto);
        Task DeleteAsync(int id);
    }
}
