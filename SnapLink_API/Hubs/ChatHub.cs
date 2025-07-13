using Microsoft.AspNetCore.SignalR;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;

namespace SnapLink_API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private static readonly Dictionary<int, string> _userConnections = new Dictionary<int, string>();

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            // For now, we'll use a simple approach - user ID will be passed in connection
            // TODO: Implement proper authentication later
            await base.OnConnectedAsync();
        }
// TODO: Implement proper user tracking when authentication is added
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Remove user from connection tracking
            var userIdToRemove = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userIdToRemove != 0)
            {
                _userConnections.Remove(userIdToRemove);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userIdToRemove}");
                
                // Notify others that user is offline
                await Clients.OthersInGroup($"User_{userIdToRemove}").SendAsync("UserOffline", userIdToRemove);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(int userId, int otherUserId)
        {
            // Store user connection
            _userConnections[userId] = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            
            // Join a group for this conversation
            var groupName = GetConversationGroupName(userId, otherUserId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            // Notify others that user is online
            await Clients.OthersInGroup($"User_{userId}").SendAsync("UserOnline", userId);
        }

        public async Task LeaveChat(int userId, int otherUserId)
        {
            // Leave the conversation group
            var groupName = GetConversationGroupName(userId, otherUserId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
// TODO: Implement proper user tracking when authentication is added

       public async Task SendMessage(int senderId, int recipientId, string content, string? attachmentUrl = null)
        {
            // Send message through service
            var request = new SnapLink_Model.DTO.Request.SendMessageRequest
            {
                RecipientId = recipientId,
                Content = content,
                AttachmentUrl = attachmentUrl
            };

            var response = await _chatService.SendMessageAsync(request, senderId);
            
            if (response.Error == 0 && response.Data != null)
            {
                // Send to conversation group
                var groupName = GetConversationGroupName(senderId, recipientId);
                await Clients.Group(groupName).SendAsync("ReceiveMessage", response.Data);
                
                // Send notification to recipient if not in the same group
                if (_userConnections.ContainsKey(recipientId))
                {
                    await Clients.Client(_userConnections[recipientId]).SendAsync("NewMessageNotification", response.Data);
                }
            }
        }

        public async Task MarkMessageAsRead(int userId, int messageId)
        {
            var response = await _chatService.MarkMessageAsReadAsync(messageId, userId);
            
            if (response.Error == 0 && response.Data != null)
            {
                // Notify sender that message was read
                var senderId = response.Data.SenderId;
                if (_userConnections.ContainsKey(senderId))
                {
                    await Clients.Client(_userConnections[senderId]).SendAsync("MessageRead", messageId);
                }
            }
        }

        public async Task MarkAllMessagesAsRead(int userId, int otherUserId)
        {
            var success = await _chatService.MarkAllMessagesAsReadAsync(userId, otherUserId);
            
            if (success)
            {
                // Notify the other user that all messages were read
                if (_userConnections.ContainsKey(otherUserId))
                {
                    await Clients.Client(_userConnections[otherUserId]).SendAsync("AllMessagesRead", userId);
                }
            }
        }

        public async Task TypingIndicator(int senderId, int recipientId, bool isTyping)
        {
            // Send typing indicator to recipient
            if (_userConnections.ContainsKey(recipientId))
            {
                await Clients.Client(_userConnections[recipientId]).SendAsync("TypingIndicator", senderId, isTyping);
            }
        }

        public async Task GetOnlineStatus(int userId)
        {
            var isOnline = _userConnections.ContainsKey(userId);
            await Clients.Caller.SendAsync("OnlineStatus", userId, isOnline);
        }

        public async Task DisconnectUser(int userId)
        {
            if (_userConnections.ContainsKey(userId))
            {
                _userConnections.Remove(userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                
                // Notify others that user is offline
                await Clients.OthersInGroup($"User_{userId}").SendAsync("UserOffline", userId);
            }
        }

        private string GetConversationGroupName(int user1Id, int user2Id)
        {
            // Create a consistent group name regardless of sender/recipient order
            var sortedIds = new[] { user1Id, user2Id }.OrderBy(id => id).ToArray();
            return $"Conversation_{sortedIds[0]}_{sortedIds[1]}";
        }

        public static bool IsUserOnline(int userId)
        {
            return _userConnections.ContainsKey(userId);
        }

        public static string? GetUserConnectionId(int userId)
        {
            return _userConnections.TryGetValue(userId, out string? connectionId) ? connectionId : null;
        }
    }
} 