using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Send a message to another user
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request, [FromQuery] int userId)
        {
            var response = await _chatService.SendMessageAsync(request, userId);
            
            if (response.Error == 0)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Get conversation messages between two users
        /// </summary>
        [HttpGet("conversation/{otherUserId}")]
        public async Task<IActionResult> GetConversationMessages(int otherUserId, [FromQuery] int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var response = await _chatService.GetConversationMessagesAsync(userId, otherUserId, page, pageSize);
            
            if (response.Error == 0)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Get all conversations for the current user
        /// </summary>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetUserConversations([FromQuery] int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _chatService.GetUserConversationsAsync(userId, page, pageSize);
            
            if (response.Error == 0)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Mark a specific message as read
        /// </summary>
        [HttpPut("messages/{messageId}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int messageId, [FromQuery] int userId)
        {
            var response = await _chatService.MarkMessageAsReadAsync(messageId, userId);
            
            if (response.Error == 0)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Mark all messages from a specific user as read
        /// </summary>
        [HttpPut("conversation/{otherUserId}/read-all")]
        public async Task<IActionResult> MarkAllMessagesAsRead(int otherUserId, [FromQuery] int userId)
        {
            var success = await _chatService.MarkAllMessagesAsReadAsync(userId, otherUserId);
            
            if (success)
            {
                return Ok(new { Error = 0, Message = "All messages marked as read successfully" });
            }
            
            return BadRequest(new { Error = -1, Message = "Failed to mark messages as read" });
        }

        /// <summary>
        /// Get unread message count for the current user
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadMessageCount([FromQuery] int userId)
        {
            var response = await _chatService.GetUnreadMessageCountAsync(userId);
            
            if (response.Error == 0)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Get a specific message by ID
        /// </summary>
        [HttpGet("messages/{messageId}")]
        public async Task<IActionResult> GetMessageById(int messageId)
        {
            var response = await _chatService.GetMessageByIdAsync(messageId);
            
            if (response.Error == 0)
            {
                return Ok(response);
            }
            
            return NotFound(response);
        }

        /// <summary>
        /// Get unread message count for a specific user
        /// </summary>
        [HttpGet("unread-count/{targetUserId}")]
        public async Task<IActionResult> GetUnreadMessageCountForUser(int targetUserId, [FromQuery] int userId)
        {
            var response = await _chatService.GetUnreadMessageCountAsync(targetUserId);
            
            if (response.Error == 0)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }
    }
} 