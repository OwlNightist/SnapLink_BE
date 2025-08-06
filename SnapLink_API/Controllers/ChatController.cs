using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Service.IService;
using System.Security.Claims;

namespace SnapLink_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        #region Message Endpoints

        /// <summary>
        /// Send a message to a user or conversation
        /// </summary>
        [HttpPost("send-message")]
        public async Task<ActionResult<SendMessageResponse>> SendMessage([FromBody] SendMessageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int senderId))
            {
                return Unauthorized("Invalid token or user not found");
            }

            // Create request with sender ID from token
            var sendMessageRequest = new SendMessageRequest
            {
                RecipientId = request.RecipientId,
                Content = request.Content,
                MessageType = request.MessageType,
                ConversationId = request.ConversationId
            };

            var result = await _chatService.SendMessageAsync(sendMessageRequest, senderId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get a specific message by ID
        /// </summary>
        [HttpGet("messages/{messageId}")]
        public async Task<ActionResult<MessageResponse>> GetMessage(int messageId)
        {
            var message = await _chatService.GetMessageByIdAsync(messageId);
            
            if (message == null)
            {
                return NotFound("Message not found");
            }

            return Ok(message);
        }

        /// <summary>
        /// Get messages from a conversation with pagination
        /// </summary>
        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<ActionResult<GetMessagesResponse>> GetConversationMessages(
            int conversationId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20)
        {
            var request = new GetConversationMessagesRequest
            {
                ConversationId = conversationId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _chatService.GetConversationMessagesAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Mark a message as read
        /// </summary>
        [HttpPost("messages/{messageId}/mark-read")]
        public async Task<ActionResult> MarkMessageAsRead(int messageId, [FromBody] MarkMessageAsReadRequest request)
        {
            if (request.MessageId != messageId)
            {
                return BadRequest("Message ID mismatch");
            }

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid token or user not found");
            }

            var success = await _chatService.MarkMessageAsReadAsync(request, userId);
            
            if (!success)
            {
                return BadRequest("Failed to mark message as read");
            }

            return Ok(new { Success = true, Message = "Message marked as read" });
        }

        /// <summary>
        /// Delete a message (only by sender)
        /// </summary>
        [HttpDelete("messages/{messageId}")]
        public async Task<ActionResult> DeleteMessage(int messageId)
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid token or user not found");
            }

            var success = await _chatService.DeleteMessageAsync(messageId, userId);
            
            if (!success)
            {
                return BadRequest("Failed to delete message");
            }

            return Ok(new { Success = true, Message = "Message deleted successfully" });
        }

        #endregion

        #region Conversation Endpoints

        /// <summary>
        /// Create a new conversation
        /// </summary>
        [HttpPost("conversations")]
        public async Task<ActionResult<CreateConversationResponse>> CreateConversation([FromBody] CreateConversationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _chatService.CreateConversationAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get a specific conversation by ID
        /// </summary>
        [HttpGet("conversations/{conversationId}")]
        public async Task<ActionResult<ConversationResponse>> GetConversation(int conversationId)
        {
            var conversation = await _chatService.GetConversationByIdAsync(conversationId);
            
            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            return Ok(conversation);
        }

        /// <summary>
        /// Get all conversations for the authenticated user
        /// </summary>
        [HttpGet("conversations")]
        public async Task<ActionResult<GetConversationsResponse>> GetUserConversations(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20)
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid token or user not found");
            }

            var result = await _chatService.GetUserConversationsAsync(userId, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Update conversation details
        /// </summary>
        [HttpPut("conversations/{conversationId}")]
        public async Task<ActionResult> UpdateConversation(
            int conversationId, 
            [FromQuery] string title, 
            [FromQuery] string status)
        {
            var success = await _chatService.UpdateConversationAsync(conversationId, title, status);
            
            if (!success)
            {
                return BadRequest("Failed to update conversation");
            }

            return Ok(new { Success = true, Message = "Conversation updated successfully" });
        }

        /// <summary>
        /// Delete/Leave a conversation
        /// </summary>
        [HttpDelete("conversations/{conversationId}")]
        public async Task<ActionResult> DeleteConversation(int conversationId)
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid token or user not found");
            }

            var success = await _chatService.DeleteConversationAsync(conversationId, userId);
            
            if (!success)
            {
                return BadRequest("Failed to delete conversation");
            }

            return Ok(new { Success = true, Message = "Conversation deleted successfully" });
        }

        #endregion

        #region Participant Endpoints

        /// <summary>
        /// Add a participant to a conversation
        /// </summary>
        [HttpPost("conversations/{conversationId}/participants")]
        public async Task<ActionResult> AddParticipant(int conversationId, [FromBody] AddParticipantRequest request)
        {
            if (request.ConversationId != conversationId)
            {
                return BadRequest("Conversation ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _chatService.AddParticipantAsync(request);
            
            if (!success)
            {
                return BadRequest("Failed to add participant");
            }

            return Ok(new { Success = true, Message = "Participant added successfully" });
        }

        /// <summary>
        /// Remove a participant from a conversation
        /// </summary>
        [HttpDelete("conversations/{conversationId}/participants")]
        public async Task<ActionResult> RemoveParticipant(int conversationId, [FromBody] RemoveParticipantRequest request)
        {
            if (request.ConversationId != conversationId)
            {
                return BadRequest("Conversation ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _chatService.RemoveParticipantAsync(request);
            
            if (!success)
            {
                return BadRequest("Failed to remove participant");
            }

            return Ok(new { Success = true, Message = "Participant removed successfully" });
        }

        /// <summary>
        /// Get all participants in a conversation
        /// </summary>
        [HttpGet("conversations/{conversationId}/participants")]
        public async Task<ActionResult<List<ConversationParticipantResponse>>> GetConversationParticipants(int conversationId)
        {
            var participants = await _chatService.GetConversationParticipantsAsync(conversationId);
            return Ok(participants);
        }

        /// <summary>
        /// Leave a conversation
        /// </summary>
        [HttpPost("conversations/{conversationId}/leave")]
        public async Task<ActionResult> LeaveConversation(int conversationId)
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid token or user not found");
            }

            var success = await _chatService.LeaveConversationAsync(conversationId, userId);
            
            if (!success)
            {
                return BadRequest("Failed to leave conversation");
            }

            return Ok(new { Success = true, Message = "Left conversation successfully" });
        }

        #endregion

        #region Utility Endpoints

        /// <summary>
        /// Get unread message count for a conversation
        /// </summary>
        [HttpGet("conversations/{conversationId}/unread-count")]
        public async Task<ActionResult<int>> GetUnreadMessageCount(int conversationId)
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid token or user not found");
            }

            var count = await _chatService.GetUnreadMessageCountAsync(userId, conversationId);
            return Ok(count);
        }

        /// <summary>
        /// Check if user is in a conversation
        /// </summary>
        [HttpGet("conversations/{conversationId}/is-participant")]
        public async Task<ActionResult<bool>> IsUserInConversation(int conversationId)
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid token or user not found");
            }

            var isParticipant = await _chatService.IsUserInConversationAsync(userId, conversationId);
            return Ok(isParticipant);
        }

        /// <summary>
        /// Get or create direct conversation between authenticated user and another user
        /// </summary>
        [HttpGet("direct-conversation")]
        public async Task<ActionResult<ConversationResponse>> GetOrCreateDirectConversation(
            [FromQuery] int otherUserId)
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                return Unauthorized("Invalid token or user not found");
            }

            var conversation = await _chatService.GetOrCreateDirectConversationAsync(currentUserId, otherUserId);
            
            if (conversation == null)
            {
                return BadRequest("Failed to get or create direct conversation");
            }

            return Ok(conversation);
        }

        #endregion
    }
} 