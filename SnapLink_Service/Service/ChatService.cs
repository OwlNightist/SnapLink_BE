using Microsoft.EntityFrameworkCore;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class ChatService : IChatService
    {
        private readonly SnaplinkDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(SnaplinkDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<ChatResponse> SendMessageAsync(SendMessageRequest request, int senderId)
        {
            try
            {
                // Validate sender exists
                var sender = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == senderId);
                
                if (sender == null)
                {
                    return new ChatResponse
                    {
                        Error = -1,
                        Message = "Sender not found",
                        Data = null
                    };
                }

                // Validate recipient exists
                var recipient = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == request.RecipientId);
                
                if (recipient == null)
                {
                    return new ChatResponse
                    {
                        Error = -1,
                        Message = "Recipient not found",
                        Data = null
                    };
                }

                // Create message
                var message = new Messagess
                {
                    SenderId = senderId,
                    RecipientId = request.RecipientId,
                    Content = request.Content,
                    AttachmentUrl = request.AttachmentUrl,
                    ReadStatus = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.MessagessRepository.AddAsync(message);
                await _unitOfWork.SaveChangesAsync();

                // Map to response
                var messageData = await MapMessageToResponseAsync(message);
                
                return new ChatResponse
                {
                    Error = 0,
                    Message = "Message sent successfully",
                    Data = messageData
                };
            }
            catch (Exception ex)
            {
                return new ChatResponse
                {
                    Error = -1,
                    Message = $"Failed to send message: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ChatListResponse> GetConversationMessagesAsync(int userId, int otherUserId, int page = 1, int pageSize = 20)
        {
            try
            {
                // Get messages between the two users
                var query = _context.Messagesses
                    .Include(m => m.Sender)
                    .Include(m => m.Recipient)
                    .Where(m => (m.SenderId == userId && m.RecipientId == otherUserId) ||
                               (m.SenderId == otherUserId && m.RecipientId == userId))
                    .OrderByDescending(m => m.CreatedAt);

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var messages = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .OrderBy(m => m.CreatedAt) // Show oldest first for chat
                    .ToListAsync();

                var messageDataList = new List<MessageData>();
                foreach (var message in messages)
                {
                    messageDataList.Add(await MapMessageToResponseAsync(message));
                }

                return new ChatListResponse
                {
                    Error = 0,
                    Message = "Conversation messages retrieved successfully",
                    Data = new ChatListData
                    {
                        Messages = messageDataList,
                        TotalCount = totalCount,
                        Page = page,
                        PageSize = pageSize,
                        TotalPages = totalPages
                    }
                };
            }
            catch (Exception ex)
            {
                return new ChatListResponse
                {
                    Error = -1,
                    Message = $"Failed to get conversation messages: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ConversationListResponse> GetUserConversationsAsync(int userId, int page = 1, int pageSize = 10)
        {
            try
            {
                // Get all unique conversations for the user
                var conversations = await _context.Messagesses
                    .Where(m => m.SenderId == userId || m.RecipientId == userId)
                    .GroupBy(m => m.SenderId == userId ? m.RecipientId : m.SenderId)
                    .Select(g => new
                    {
                        OtherUserId = g.Key,
                        LastMessage = g.OrderByDescending(m => m.CreatedAt).First(),
                        UnreadCount = g.Count(m => m.RecipientId == userId && (m.ReadStatus != true))
                    })
                    .OrderByDescending(c => c.LastMessage.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var conversationDataList = new List<ConversationData>();
                foreach (var conv in conversations)
                {
                    var otherUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.UserId == conv.OtherUserId);

                    if (otherUser != null)
                    {
                        conversationDataList.Add(new ConversationData
                        {
                            OtherUserId = conv.OtherUserId.Value,
                            OtherUserName = otherUser.FullName ?? otherUser.UserName ?? "",
                            OtherUserEmail = otherUser.Email ?? "",
                            OtherUserProfileImage = otherUser.ProfileImage,
                            LastMessage = conv.LastMessage.Content ?? "",
                            LastMessageTime = conv.LastMessage.CreatedAt ?? DateTime.UtcNow,
                            UnreadCount = conv.UnreadCount,
                            IsOnline = false // TODO: Implement online status tracking
                        });
                    }
                }

                var totalCount = await _context.Messagesses
                    .Where(m => m.SenderId == userId || m.RecipientId == userId)
                    .GroupBy(m => m.SenderId == userId ? m.RecipientId : m.SenderId)
                    .CountAsync();

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return new ConversationListResponse
                {
                    Error = 0,
                    Message = "User conversations retrieved successfully",
                    Data = new ConversationListData
                    {
                        Conversations = conversationDataList,
                        TotalCount = totalCount,
                        Page = page,
                        PageSize = pageSize,
                        TotalPages = totalPages
                    }
                };
            }
            catch (Exception ex)
            {
                return new ConversationListResponse
                {
                    Error = -1,
                    Message = $"Failed to get user conversations: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ChatResponse> MarkMessageAsReadAsync(int messageId, int userId)
        {
            try
            {
                var message = await _context.Messagesses
                    .FirstOrDefaultAsync(m => m.MessageId == messageId);

                if (message == null)
                {
                    return new ChatResponse
                    {
                        Error = -1,
                        Message = "Message not found",
                        Data = null
                    };
                }

                // Only recipient can mark message as read
                if (message.RecipientId != userId)
                {
                    return new ChatResponse
                    {
                        Error = -1,
                        Message = "You can only mark messages sent to you as read",
                        Data = null
                    };
                }

                message.ReadStatus = true;
                await _unitOfWork.SaveChangesAsync();

                var messageData = await MapMessageToResponseAsync(message);
                
                return new ChatResponse
                {
                    Error = 0,
                    Message = "Message marked as read successfully",
                    Data = messageData
                };
            }
            catch (Exception ex)
            {
                return new ChatResponse
                {
                    Error = -1,
                    Message = $"Failed to mark message as read: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<UnreadCountResponse> GetUnreadMessageCountAsync(int userId)
        {
            try
            {
                var unreadMessages = await _context.Messagesses
                    .Where(m => m.RecipientId == userId && (m.ReadStatus != true))
                    .GroupBy(m => m.SenderId)
                    .Select(g => new { SenderId = g.Key, Count = g.Count() })
                    .ToListAsync();

                var totalUnreadCount = unreadMessages.Sum(u => u.Count);
                var unreadCountByUser = unreadMessages.ToDictionary(u => u.SenderId.Value, u => u.Count);

                return new UnreadCountResponse
                {
                    Error = 0,
                    Message = "Unread message count retrieved successfully",
                    Data = new UnreadCountData
                    {
                        TotalUnreadCount = totalUnreadCount,
                        UnreadCountByUser = unreadCountByUser
                    }
                };
            }
            catch (Exception ex)
            {
                return new UnreadCountResponse
                {
                    Error = -1,
                    Message = $"Failed to get unread message count: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<bool> MarkAllMessagesAsReadAsync(int userId, int otherUserId)
        {
            try
            {
                var unreadMessages = await _context.Messagesses
                    .Where(m => m.RecipientId == userId && 
                               m.SenderId == otherUserId && 
                               (m.ReadStatus != true))
                    .ToListAsync();

                foreach (var message in unreadMessages)
                {
                    message.ReadStatus = true;
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ChatResponse> GetMessageByIdAsync(int messageId)
        {
            try
            {
                var message = await _context.Messagesses
                    .Include(m => m.Sender)
                    .Include(m => m.Recipient)
                    .FirstOrDefaultAsync(m => m.MessageId == messageId);

                if (message == null)
                {
                    return new ChatResponse
                    {
                        Error = -1,
                        Message = "Message not found",
                        Data = null
                    };
                }

                var messageData = await MapMessageToResponseAsync(message);
                
                return new ChatResponse
                {
                    Error = 0,
                    Message = "Message retrieved successfully",
                    Data = messageData
                };
            }
            catch (Exception ex)
            {
                return new ChatResponse
                {
                    Error = -1,
                    Message = $"Failed to get message: {ex.Message}",
                    Data = null
                };
            }
        }

        private async Task<MessageData> MapMessageToResponseAsync(Messagess message)
        {
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserId == message.SenderId);
            var recipient = await _context.Users.FirstOrDefaultAsync(u => u.UserId == message.RecipientId);

            return new MessageData
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId ?? 0,
                SenderName = sender?.FullName ?? sender?.UserName ?? "",
                SenderEmail = sender?.Email ?? "",
                SenderProfileImage = sender?.ProfileImage,
                RecipientId = message.RecipientId ?? 0,
                RecipientName = recipient?.FullName ?? recipient?.UserName ?? "",
                RecipientEmail = recipient?.Email ?? "",
                RecipientProfileImage = recipient?.ProfileImage,
                Content = message.Content ?? "",
                AttachmentUrl = message.AttachmentUrl,
                ReadStatus = message.ReadStatus ?? false,
                CreatedAt = message.CreatedAt ?? DateTime.UtcNow
            };
        }
    }
} 