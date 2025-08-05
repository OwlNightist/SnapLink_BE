using Microsoft.AspNetCore.SignalR;
using SnapLink_Model.DTO.Response;

namespace SnapLink_API.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, int> _userConnections = new Dictionary<string, int>();

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
            await Clients.Group($"conversation_{conversationId}").SendAsync("ReceiveMessage", message);
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
    }
} 