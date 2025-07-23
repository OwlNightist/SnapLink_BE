using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class AddUserStyleRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int StyleId { get; set; }
    }

    public class UpdateUserStylesRequest
    {
        [Required]
        public int UserId { get; set; }

        public List<int> StyleIds { get; set; } = new List<int>();
    }
} 