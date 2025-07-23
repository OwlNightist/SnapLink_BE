using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IChatService
    {
        Task<ChatResponse> SendMessageAsync(SendMessageRequest request, int senderId);
        Task<ChatListResponse> GetConversationMessagesAsync(int userId, int otherUserId, int page = 1, int pageSize = 20);
        Task<ConversationListResponse> GetUserConversationsAsync(int userId, int page = 1, int pageSize = 10);
        Task<ChatResponse> MarkMessageAsReadAsync(int messageId, int userId);
        Task<UnreadCountResponse> GetUnreadMessageCountAsync(int userId);
        Task<bool> MarkAllMessagesAsReadAsync(int userId, int otherUserId);
        Task<ChatResponse> GetMessageByIdAsync(int messageId);
    }
} 