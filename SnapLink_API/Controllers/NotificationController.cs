using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }
        [HttpGet("GetAllNotifications")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("GetNotificationById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("GetNotificationsByUserId/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpPost("CreateNotification")]
        public async Task<IActionResult> Create(NotificationDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok("Created");
        }

        [HttpPut("UpdateNotification/{id}")]
        public async Task<IActionResult> Update(int id, NotificationDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Updated");
        }

        [HttpDelete("DeleteNotification/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok("Deleted");
        }
    }
}
