using Microsoft.AspNetCore.SignalR;
using SnapLink_Model.DTO.Response;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;

namespace SnapLink_API.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, int> _userConnections = new Dictionary<string, int>();
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Remove user from connection mapping
            if (_userConnections.ContainsKey(Context.ConnectionId))
            {
                _userConnections.Remove(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Register user connection with their user ID
        /// </summary>
        public async Task RegisterUser(int userId)
        {
            _userConnections[Context.ConnectionId] = userId;
            
            // Add user to a group for their user ID
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            
            await Clients.Caller.SendAsync("UserRegistered", userId);
        }

        /// <summary>
        /// Join a conversation group
        /// </summary>
        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            await Clients.Caller.SendAsync("JoinedConversation", conversationId);
        }

        /// <summary>
        /// Leave a conversation group
        /// </summary>
        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            await Clients.Caller.SendAsync("LeftConversation", conversationId);
        }

        /// <summary>
        /// Send a message to a conversation
        /// </summary>
        public async Task SendMessageToConversation(int conversationId, MessageResponse message)
        {
            try
            {
                // Validate message
                if (message == null || string.IsNullOrWhiteSpace(message.Content))
                {
                    await Clients.Caller.SendAsync("Error", "Invalid message");
                    return;
                }

                // Validate conversation ID
                if (conversationId <= 0)
                {
                    await Clients.Caller.SendAsync("Error", "Invalid conversation ID");
                    return;
                }

                // Get current user ID
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue || currentUserId.Value != message.SenderId)
                {
                    await Clients.Caller.SendAsync("Error", "Unauthorized to send message");
                    return;
                }

                // Persist message to database
                var sendMessageRequest = new SendMessageRequest
                {
                    RecipientId = message.RecipientId ?? 0,
                    Content = message.Content,
                    MessageType = message.MessageType ?? "Text",
                    ConversationId = conversationId
                };

                var result = await _chatService.SendMessageAsync(sendMessageRequest, currentUserId.Value);
                
                if (!result.Success)
                {
                    await Clients.Caller.SendAsync("Error", $"Failed to persist message: {result.Message}");
                    return;
                }

                // Use the persisted message data for broadcasting
                var persistedMessage = result.MessageData;
                if (persistedMessage != null)
                {
                    // Broadcast message to conversation group EXCEPT the sender
                    await Clients.GroupExcept($"conversation_{conversationId}", Context.ConnectionId)
                        .SendAsync("ReceiveMessage", persistedMessage);
                    
                    // Send confirmation to the sender with persisted data
                    await Clients.Caller.SendAsync("MessageSent", persistedMessage);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Failed to send message: {ex.Message}");
            }
        }

        /// <summary>
        /// Send a message to a specific user
        /// </summary>
        public async Task SendMessageToUser(int recipientUserId, MessageResponse message)
        {
            await Clients.Group($"user_{recipientUserId}").SendAsync("ReceiveMessage", message);
        }

        /// <summary>
        /// Notify users about new conversation
        /// </summary>
        public async Task NotifyNewConversation(int userId, ConversationResponse conversation)
        {
            await Clients.Group($"user_{userId}").SendAsync("NewConversation", conversation);
        }

        /// <summary>
        /// Notify users about conversation updates
        /// </summary>
        public async Task NotifyConversationUpdate(int conversationId, object update)
        {
            await Clients.Group($"conversation_{conversationId}").SendAsync("ConversationUpdated", update);
        }

        /// <summary>
        /// Notify users about message status changes
        /// </summary>
        public async Task NotifyMessageStatus(int conversationId, int messageId, string status)
        {
            await Clients.Group($"conversation_{conversationId}").SendAsync("MessageStatusChanged", messageId, status);
        }

        /// <summary>
        /// Get current user ID for this connection
        /// </summary>
        public int? GetCurrentUserId()
        {
            return _userConnections.TryGetValue(Context.ConnectionId, out int userId) ? userId : null;
        }

        /// <summary>
        /// Get all connected users
        /// </summary>
        public List<int> GetConnectedUsers()
        {
            return _userConnections.Values.Distinct().ToList();
        }

        /// <summary>
        /// Check if a user is online
        /// </summary>
        public bool IsUserOnline(int userId)
        {
            return _userConnections.Values.Contains(userId);
        }

        /// <summary>
        /// Send typing indicator to conversation participants
        /// </summary>
        public async Task SendTypingIndicator(int conversationId, int userId, bool isTyping)
        {
            await Clients.Group($"conversation_{conversationId}").SendAsync("UserTyping", userId, isTyping);
        }
    }
} 