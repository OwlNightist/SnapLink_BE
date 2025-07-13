using System.ComponentModel.DataAnnotations;

namespace SnapLink_Model.DTO.Response
{
    public class ChatResponse
    {
        public int Error { get; set; }
        public string Message { get; set; } = string.Empty;
        public MessageData? Data { get; set; }
    }

    public class ChatListResponse
    {
        public int Error { get; set; }
        public string Message { get; set; } = string.Empty;
        public ChatListData? Data { get; set; }
    }

    public class ConversationListResponse
    {
        public int Error { get; set; }
        public string Message { get; set; } = string.Empty;
        public ConversationListData? Data { get; set; }
    }

    public class UnreadCountResponse
    {
        public int Error { get; set; }
        public string Message { get; set; } = string.Empty;
        public UnreadCountData? Data { get; set; }
    }

    public class MessageData
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string? SenderProfileImage { get; set; }
        public int RecipientId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string? RecipientProfileImage { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
        public bool ReadStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ChatListData
    {
        public List<MessageData> Messages { get; set; } = new List<MessageData>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class ConversationData
    {
        public int OtherUserId { get; set; }
        public string OtherUserName { get; set; } = string.Empty;
        public string OtherUserEmail { get; set; } = string.Empty;
        public string? OtherUserProfileImage { get; set; }
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public bool IsOnline { get; set; }
    }

    public class ConversationListData
    {
        public List<ConversationData> Conversations { get; set; } = new List<ConversationData>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class UnreadCountData
    {
        public int TotalUnreadCount { get; set; }
        public Dictionary<int, int> UnreadCountByUser { get; set; } = new Dictionary<int, int>();
    }
} 