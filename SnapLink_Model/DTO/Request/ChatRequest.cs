using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Request
{
    public class SendMessageRequest
    {
        [Required]
        public int RecipientId { get; set; }

        [Required]
        [MaxLength(400)]
        public string Content { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? AttachmentUrl { get; set; }
    }

    public class GetConversationMessagesRequest
    {
        [Required]
        public int OtherUserId { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class MarkMessageAsReadRequest
    {
        [Required]
        public int MessageId { get; set; }
    }
} 