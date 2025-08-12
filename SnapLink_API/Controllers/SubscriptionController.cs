using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _svc;
        public SubscriptionController(ISubscriptionService svc) => _svc = svc;

        [HttpPost("Subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribePackageDto dto)
        {
            try { var sub = await _svc.SubscribeAsync(dto); return Ok(sub); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("Photographer/{photographerId:int}")]
        public async Task<IActionResult> GetByPhotographer(int photographerId) =>
            Ok(await _svc.GetByPhotographerAsync(photographerId));

        [HttpGet("Location/{locationId:int}")]
        public async Task<IActionResult> GetByLocation(int locationId) =>
            Ok(await _svc.GetByLocationAsync(locationId));

        [HttpGet("subscribers")]
        public async Task<IActionResult> GetAllSubscribers([FromQuery] string? status = "Active")
        {
            var data = await _svc.GetAllSubscribersAsync(status);
            return Ok(data);
        }
        [HttpGet("subscribers/photographers")]
        public async Task<IActionResult> GetAllPhotographerSubscribers([FromQuery] string? status = "Active")
        {
            var data = await _svc.GetAllPhotographerSubscribersAsync(status);
            return Ok(data);
        }
        [HttpGet("subscribers/locations")]
        public async Task<IActionResult> GetAllLocationSubscribers([FromQuery] string? status = "Active")
        {
            var data = await _svc.GetAllLocationSubscribersAsync(status);
            return Ok(data);
        }
        [HttpPut("{subscriptionId:int}/cancel")]
        public async Task<IActionResult> Cancel(int subscriptionId, [FromQuery] string? reason = null)
        {
            try { return Ok(new { message = await _svc.CancelAsync(subscriptionId, reason) }); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        /// <summary>
        /// Chạy quét hết hạn ngay (admin/troubleshoot). Có thể gắn [Authorize(Roles="Admin")]
        /// </summary>
        [HttpPost("expire-run-now")]
        public async Task<IActionResult> ExpireRunNow()
        {
            try { var n = await _svc.ExpireOverduesAsync(); return Ok(new { expired = n }); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
