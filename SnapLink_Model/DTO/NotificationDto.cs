using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Model.DTO
{
    public class NotificationDto
    {
        public int UserId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? NotificationType { get; set; }
        public int? ReferenceId { get; set; }
        public bool? ReadStatus { get; set; }
    }
}
