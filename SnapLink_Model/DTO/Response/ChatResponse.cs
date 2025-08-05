using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Response
{
    public class MessageResponse
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int? RecipientId { get; set; }
        public int? ConversationId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? MessageType { get; set; }
        public string? Status { get; set; } // "sent", "read"
        public DateTime? ReadAt { get; set; }
        public string? SenderName { get; set; }
        public string? SenderProfileImage { get; set; }
    }

    public class ConversationResponse
    {
        public int ConversationId { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public List<ConversationParticipantResponse> Participants { get; set; } = new List<ConversationParticipantResponse>();
        public MessageResponse? LastMessage { get; set; }
        public int UnreadCount { get; set; }
    }

    public class ConversationParticipantResponse
    {
        public int ConversationParticipantId { get; set; }
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
        public string? UserName { get; set; }
        public string? UserProfileImage { get; set; }
        public string? UserFullName { get; set; }
    }

    public class SendMessageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public MessageResponse? MessageData { get; set; }
        public int? ConversationId { get; set; }
    }

    public class CreateConversationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ConversationResponse? Conversation { get; set; }
    }

    public class GetConversationsResponse
    {
        public List<ConversationResponse> Conversations { get; set; } = new List<ConversationResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class GetMessagesResponse
    {
        public List<MessageResponse> Messages { get; set; } = new List<MessageResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }
    }

    public class ChatApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
} 