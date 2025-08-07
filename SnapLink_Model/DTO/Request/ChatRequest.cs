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

        public string? MessageType { get; set; } = "Text"; // Text, Image, File, etc.

        public int? ConversationId { get; set; } // Optional, will create new conversation if not provided
    }

    public class CreateConversationRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = "Direct"; // Direct, Group

        [Required]
        public List<int> ParticipantIds { get; set; } = new List<int>();

        public string? Status { get; set; } = "Active";
    }

    public class AddParticipantRequest
    {
        [Required]
        public int ConversationId { get; set; }

        [Required]
        public int UserId { get; set; }

        public string? Role { get; set; } = "Member"; // Admin, Member
    }

    public class RemoveParticipantRequest
    {
        [Required]
        public int ConversationId { get; set; }

        [Required]
        public int UserId { get; set; }
    }

    public class MarkMessageAsReadRequest
    {
        [Required]
        public int MessageId { get; set; }
    }

    public class GetConversationMessagesRequest
    {
        [Required]
        public int ConversationId { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class TypingIndicatorRequest
    {
        [Required]
        public bool IsTyping { get; set; }
    }
} 