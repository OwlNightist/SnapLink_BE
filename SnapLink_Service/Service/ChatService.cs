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
        private readonly IPushNotificationService _pushNotificationService;

        public ChatService(SnaplinkDbContext context, IUnitOfWork unitOfWork, IPushNotificationService pushNotificationService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _pushNotificationService = pushNotificationService;
        }

        #region Message Operations

        public async Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request, int senderId)
        {
            try
            {
                // Validate sender and recipient exist
                var sender = await _context.Users.FindAsync(senderId);
                var recipient = await _context.Users.FindAsync(request.RecipientId);

                if (sender == null || recipient == null)
                {
                    return new SendMessageResponse
                    {
                        Success = false,
                        Message = "Sender or recipient not found"
                    };
                }
                if (sender == recipient)
                {
                    return new SendMessageResponse
                    {
                        Success = false,
                        Message = "Dont Fuck your self"
                    };
                }
                // Get or create conversation
                int conversationId;
                // First, try to find existing 1-to-1 conversation with the recipient
                var existingDirectConversation = await _context.ConversationParticipants
                    .Include(cp => cp.Conversation)
                        .ThenInclude(c => c.Participants)
                    .Where(cp => cp.UserId == senderId && cp.IsActive)
                    .Select(cp => cp.Conversation)
                    .Where(c => c.Type == "Direct" && c.Status == "Active")
                    .FirstOrDefaultAsync(c => c.Participants.Any(p => p.UserId == request.RecipientId && p.IsActive));

                if (existingDirectConversation != null)
                {
                    // Use existing 1-to-1 conversation
                    conversationId = existingDirectConversation.ConversationId;
                }
                else if (request.ConversationId.HasValue)
                {
                    // Use specified conversation ID (for group conversations or specific cases)
                    var existingConversation = await _context.Conversations
                        .Include(c => c.Participants)
                        .FirstOrDefaultAsync(c => c.ConversationId == request.ConversationId.Value);

                    if (existingConversation == null)
                    {
                        return new SendMessageResponse
                        {
                            Success = false,
                            Message = "Conversation not found"
                        };
                    }

                    // Check if sender is participant
                    if (!existingConversation.Participants.Any(p => p.UserId == senderId && p.IsActive))
                    {
                        return new SendMessageResponse
                        {
                            Success = false,
                            Message = "You are not a participant in this conversation"
                        };
                    }

                    conversationId = request.ConversationId.Value;
                }
                else
                {
                    // Create new direct conversation
                    var conversation = new Conversation
                    {
                        Title = $"Chat between {sender.UserName} and {recipient.UserName}",
                        Type = "Direct",
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Conversations.Add(conversation);
                    await _context.SaveChangesAsync();

                    // Add participants
                    var participants = new List<ConversationParticipant>
                    {
                        new ConversationParticipant
                        {
                            ConversationId = conversation.ConversationId,
                            UserId = senderId,
                            JoinedAt = DateTime.UtcNow,
                            Role = "Member",
                            IsActive = true
                        },
                        new ConversationParticipant
                        {
                            ConversationId = conversation.ConversationId,
                            UserId = request.RecipientId,
                            JoinedAt = DateTime.UtcNow,
                            Role = "Member",
                            IsActive = true
                        }
                    };

                    _context.ConversationParticipants.AddRange(participants);
                    await _context.SaveChangesAsync();

                    conversationId = conversation.ConversationId;
                }

                // Create message
                var message = new Messagess
                {
                    SenderId = senderId,
                    RecipientId = request.RecipientId,
                    ConversationId = conversationId,
                    Content = request.Content,
                    MessageType = request.MessageType ?? "Text",
                    Status = "sent",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Messagesses.Add(message);
                await _context.SaveChangesAsync();

                // Send push notification to recipient
                try
                {
                    await _pushNotificationService.SendMessageNotificationAsync(
                        request.RecipientId,
                        sender.FullName ?? sender.UserName ?? "Unknown User",
                        request.Content,
                        conversationId
                    );
                }
                catch (Exception ex)
                {
                    // Log notification error but don't fail the message
                    Console.WriteLine($"Failed to send message notification: {ex.Message}");
                }

                // Update conversation's UpdatedAt
                var conversationToUpdate = await _context.Conversations.FindAsync(conversationId);
                if (conversationToUpdate != null)
                {
                    conversationToUpdate.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                // Map to response
                var messageResponse = new MessageResponse
                {
                    MessageId = message.MessageId,
                    SenderId = message.SenderId.Value,
                    RecipientId = message.RecipientId,
                    ConversationId = message.ConversationId,
                    Content = message.Content,
                    CreatedAt = message.CreatedAt.Value,
                    MessageType = message.MessageType,
                    Status = message.Status,
                    ReadAt = message.ReadAt,
                    SenderName = sender.UserName,
                    SenderProfileImage = sender.ProfileImage
                };

                return new SendMessageResponse
                {
                    Success = true,
                    Message = "Message sent successfully",
                    MessageData = messageResponse,
                    ConversationId = conversationId
                };
            }
            catch (Exception ex)
            {
                return new SendMessageResponse
                {
                    Success = false,
                    Message = $"Error sending message: {ex.Message}"
                };
            }
        }

        public async Task<MessageResponse?> GetMessageByIdAsync(int messageId)
        {
            var message = await _context.Messagesses
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.MessageId == messageId);

            if (message == null) return null;

            return new MessageResponse
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId.Value,
                RecipientId = message.RecipientId,
                ConversationId = message.ConversationId,
                Content = message.Content,
                CreatedAt = message.CreatedAt.Value,
                MessageType = message.MessageType,
                Status = message.Status,
                ReadAt = message.ReadAt,
                SenderName = message.Sender?.UserName,
                SenderProfileImage = message.Sender?.ProfileImage
            };
        }

        public async Task<GetMessagesResponse> GetConversationMessagesAsync(GetConversationMessagesRequest request)
        {
            var query = _context.Messagesses
                .Include(m => m.Sender)
                .Where(m => m.ConversationId == request.ConversationId && m.Status != "deleted")
                .OrderByDescending(m => m.CreatedAt);

            var totalCount = await query.CountAsync();
            var skip = (request.Page - 1) * request.PageSize;

            var messages = await query
                .Skip(skip)
                .Take(request.PageSize)
                .ToListAsync();

            var messageResponses = messages.Select(m => new MessageResponse
            {
                MessageId = m.MessageId,
                SenderId = m.SenderId.Value,
                RecipientId = m.RecipientId,
                ConversationId = m.ConversationId,
                Content = m.Content,
                CreatedAt = m.CreatedAt.Value,
                MessageType = m.MessageType,
                Status = m.Status,
                ReadAt = m.ReadAt,
                SenderName = m.Sender?.UserName,
                SenderProfileImage = m.Sender?.ProfileImage
            }).ToList();

            return new GetMessagesResponse
            {
                Messages = messageResponses,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                HasMore = skip + request.PageSize < totalCount
            };
        }

        public async Task<bool> MarkMessageAsReadAsync(int messageId, int userId)
        {
            var message = await _context.Messagesses
                .FirstOrDefaultAsync(m => m.MessageId == messageId);

            if (message == null) return false;

            // Check if user is the recipient
            if (message.RecipientId != userId) return false;

            message.Status = "read";
            message.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMessageAsync(int messageId, int userId)
        {
            var message = await _context.Messagesses
                .FirstOrDefaultAsync(m => m.MessageId == messageId);

            if (message == null) return false;

            // Check if user is the sender
            if (message.SenderId != userId) return false;

            // Change status to "deleted" instead of removing
            message.Status = "deleted";
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Conversation Operations

        public async Task<CreateConversationResponse> CreateConversationAsync(CreateConversationRequest request)
        {
            try
            {
                var conversation = new Conversation
                {
                    Title = request.Title,
                    Type = request.Type,
                    Status = request.Status ?? "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();

                // Add participants
                var participants = request.ParticipantIds.Select(userId => new ConversationParticipant
                {
                    ConversationId = conversation.ConversationId,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow,
                    Role = "Member",
                    IsActive = true
                }).ToList();

                _context.ConversationParticipants.AddRange(participants);
                await _context.SaveChangesAsync();

                var conversationResponse = await GetConversationByIdAsync(conversation.ConversationId);

                return new CreateConversationResponse
                {
                    Success = true,
                    Message = "Conversation created successfully",
                    Conversation = conversationResponse
                };
            }
            catch (Exception ex)
            {
                return new CreateConversationResponse
                {
                    Success = false,
                    Message = $"Error creating conversation: {ex.Message}"
                };
            }
        }

        public async Task<ConversationResponse?> GetConversationByIdAsync(int conversationId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

            if (conversation == null) return null;

            var participants = conversation.Participants.Select(p => new ConversationParticipantResponse
            {
                ConversationParticipantId = p.ConversationParticipantId,
                ConversationId = p.ConversationId,
                UserId = p.UserId,
                JoinedAt = p.JoinedAt.Value,
                LeftAt = p.LeftAt,
                Role = p.Role,
                IsActive = p.IsActive,
                UserName = p.User?.UserName,
                UserProfileImage = p.User?.ProfileImage,
                UserFullName = p.User?.FullName
            }).ToList();

            var lastMessage = conversation.Messages.FirstOrDefault();
            var lastMessageResponse = lastMessage != null ? new MessageResponse
            {
                MessageId = lastMessage.MessageId,
                SenderId = lastMessage.SenderId.Value,
                RecipientId = lastMessage.RecipientId,
                ConversationId = lastMessage.ConversationId,
                Content = lastMessage.Content,
                CreatedAt = lastMessage.CreatedAt.Value,
                MessageType = lastMessage.MessageType,
                Status = lastMessage.Status,
                ReadAt = lastMessage.ReadAt
            } : null;

            return new ConversationResponse
            {
                ConversationId = conversation.ConversationId,
                Title = conversation.Title,
                CreatedAt = conversation.CreatedAt.Value,
                UpdatedAt = conversation.UpdatedAt,
                Status = conversation.Status,
                Type = conversation.Type,
                Participants = participants,
                LastMessage = lastMessageResponse,
                UnreadCount = 0 // Will be calculated separately if needed
            };
        }

        public async Task<GetConversationsResponse> GetUserConversationsAsync(int userId, int page = 1, int pageSize = 20)
        {
            var query = _context.ConversationParticipants
                .Include(cp => cp.Conversation)
                    .ThenInclude(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .Include(cp => cp.Conversation)
                    .ThenInclude(c => c.Participants)
                        .ThenInclude(p => p.User)
                .Where(cp => cp.UserId == userId && cp.IsActive)
                .OrderByDescending(cp => cp.Conversation.UpdatedAt);

            var totalCount = await query.CountAsync();
            var skip = (page - 1) * pageSize;

            var userConversations = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var conversations = new List<ConversationResponse>();

            foreach (var userConversation in userConversations)
            {
                var conversation = userConversation.Conversation;
                var participants = conversation.Participants.Select(p => new ConversationParticipantResponse
                {
                    ConversationParticipantId = p.ConversationParticipantId,
                    ConversationId = p.ConversationId,
                    UserId = p.UserId,
                    JoinedAt = p.JoinedAt.Value,
                    LeftAt = p.LeftAt,
                    Role = p.Role,
                    IsActive = p.IsActive,
                    UserName = p.User?.UserName,
                    UserProfileImage = p.User?.ProfileImage,
                    UserFullName = p.User?.FullName
                }).ToList();

                var lastMessage = conversation.Messages.FirstOrDefault();
                var lastMessageResponse = lastMessage != null ? new MessageResponse
                {
                    MessageId = lastMessage.MessageId,
                    SenderId = lastMessage.SenderId.Value,
                    RecipientId = lastMessage.RecipientId,
                    ConversationId = lastMessage.ConversationId,
                    Content = lastMessage.Content,
                    CreatedAt = lastMessage.CreatedAt.Value,
                    MessageType = lastMessage.MessageType,
                    Status = lastMessage.Status,
                    ReadAt = lastMessage.ReadAt
                } : null;

                var unreadCount = await GetUnreadMessageCountAsync(userId, conversation.ConversationId);

                conversations.Add(new ConversationResponse
                {
                    ConversationId = conversation.ConversationId,
                    Title = conversation.Title,
                    CreatedAt = conversation.CreatedAt.Value,
                    UpdatedAt = conversation.UpdatedAt,
                    Status = conversation.Status,
                    Type = conversation.Type,
                    Participants = participants,
                    LastMessage = lastMessageResponse,
                    UnreadCount = unreadCount
                });
            }

            return new GetConversationsResponse
            {
                Conversations = conversations,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> UpdateConversationAsync(int conversationId, string title, string status)
        {
            var conversation = await _context.Conversations.FindAsync(conversationId);
            if (conversation == null) return false;

            conversation.Title = title;
            conversation.Status = status;
            conversation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteConversationAsync(int conversationId, int userId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

            if (conversation == null) return false;

            // Check if user is participant
            var participant = conversation.Participants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null) return false;

            // For direct conversations, mark as deleted for both participants
            if (conversation.Type == "Direct")
            {
                foreach (var p in conversation.Participants)
                {
                    p.IsActive = false;
                    p.LeftAt = DateTime.UtcNow;
                }
                conversation.Status = "Deleted";
            }
            else
            {
                // For group conversations, only remove the specific user
                participant.IsActive = false;
                participant.LeftAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Participant Operations

        public async Task<bool> AddParticipantAsync(AddParticipantRequest request)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.ConversationId == request.ConversationId);

            if (conversation == null) return false;

            // Check if user is already a participant
            if (conversation.Participants.Any(p => p.UserId == request.UserId && p.IsActive))
                return false;

            var participant = new ConversationParticipant
            {
                ConversationId = request.ConversationId,
                UserId = request.UserId,
                JoinedAt = DateTime.UtcNow,
                Role = request.Role ?? "Member",
                IsActive = true
            };

            _context.ConversationParticipants.Add(participant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveParticipantAsync(RemoveParticipantRequest request)
        {
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == request.ConversationId && p.UserId == request.UserId);

            if (participant == null) return false;

            participant.IsActive = false;
            participant.LeftAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ConversationParticipantResponse>> GetConversationParticipantsAsync(int conversationId)
        {
            var participants = await _context.ConversationParticipants
                .Include(p => p.User)
                .Where(p => p.ConversationId == conversationId && p.IsActive)
                .ToListAsync();

            return participants.Select(p => new ConversationParticipantResponse
            {
                ConversationParticipantId = p.ConversationParticipantId,
                ConversationId = p.ConversationId,
                UserId = p.UserId,
                JoinedAt = p.JoinedAt.Value,
                LeftAt = p.LeftAt,
                Role = p.Role,
                IsActive = p.IsActive,
                UserName = p.User?.UserName,
                UserProfileImage = p.User?.ProfileImage,
                UserFullName = p.User?.FullName
            }).ToList();
        }

        public async Task<bool> LeaveConversationAsync(int conversationId, int userId)
        {
            return await RemoveParticipantAsync(new RemoveParticipantRequest
            {
                ConversationId = conversationId,
                UserId = userId
            });
        }

        #endregion

        #region Utility Operations

        public async Task<int> GetUnreadMessageCountAsync(int userId, int conversationId)
        {
            return await _context.Messagesses
                .CountAsync(m => m.ConversationId == conversationId && 
                                m.RecipientId == userId && 
                                m.Status == "sent");
        }

        public async Task<bool> IsUserInConversationAsync(int userId, int conversationId)
        {
            return await _context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && 
                              p.UserId == userId && 
                              p.IsActive);
        }

        public async Task<ConversationResponse?> GetOrCreateDirectConversationAsync(int user1Id, int user2Id)
        {
            // Look for existing direct conversation between these two users
            var existingConversation = await _context.ConversationParticipants
                .Include(cp => cp.Conversation)
                    .ThenInclude(c => c.Participants)
                .Where(cp => cp.UserId == user1Id && cp.IsActive)
                .Select(cp => cp.Conversation)
                .Where(c => c.Type == "Direct" && c.Status == "Active")
                .FirstOrDefaultAsync(c => c.Participants.Any(p => p.UserId == user2Id && p.IsActive));

            if (existingConversation != null)
            {
                return await GetConversationByIdAsync(existingConversation.ConversationId);
            }

            // Create new direct conversation
            var request = new CreateConversationRequest
            {
                Title = $"Direct Chat",
                Type = "Direct",
                ParticipantIds = new List<int> { user1Id, user2Id },
                Status = "Active"
            };

            var result = await CreateConversationAsync(request);
            return result.Success ? result.Conversation : null;
        }

        #endregion
    }
} 