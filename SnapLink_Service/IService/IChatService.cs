using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IChatService
    {
        // Message operations
        Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request, int senderId);
        Task<MessageResponse?> GetMessageByIdAsync(int messageId);
        Task<GetMessagesResponse> GetConversationMessagesAsync(GetConversationMessagesRequest request);
        Task<bool> MarkMessageAsReadAsync(MarkMessageAsReadRequest request, int userId);
        Task<bool> DeleteMessageAsync(int messageId, int userId);

        // Conversation operations
        Task<CreateConversationResponse> CreateConversationAsync(CreateConversationRequest request);
        Task<ConversationResponse?> GetConversationByIdAsync(int conversationId);
        Task<GetConversationsResponse> GetUserConversationsAsync(int userId, int page = 1, int pageSize = 20);
        Task<bool> UpdateConversationAsync(int conversationId, string title, string status);
        Task<bool> DeleteConversationAsync(int conversationId, int userId);

        // Participant operations
        Task<bool> AddParticipantAsync(AddParticipantRequest request);
        Task<bool> RemoveParticipantAsync(RemoveParticipantRequest request);
        Task<List<ConversationParticipantResponse>> GetConversationParticipantsAsync(int conversationId);
        Task<bool> LeaveConversationAsync(int conversationId, int userId);

        // Utility operations
        Task<int> GetUnreadMessageCountAsync(int userId, int conversationId);
        Task<bool> IsUserInConversationAsync(int userId, int conversationId);
        Task<ConversationResponse?> GetOrCreateDirectConversationAsync(int user1Id, int user2Id);
    }
} 