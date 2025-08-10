using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _service;
        public RatingController(IRatingService service) => _service = service;

        [HttpGet("GetRatings")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("GetRatingById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _service.GetByIdAsync(id);
            return r == null ? NotFound() : Ok(r);
        }

        [HttpGet("ByPhotographer/{photographerId:int}")]
        public async Task<IActionResult> ByPhotographer(int photographerId) =>
            Ok(await _service.GetByPhotographerAsync(photographerId));

        [HttpGet("ByLocation/{locationId:int}")]
        public async Task<IActionResult> ByLocation(int locationId) =>
            Ok(await _service.GetByLocationAsync(locationId));

        // Người dùng đã đặt booking mới được tạo rating (tùy bạn thêm [Authorize])
        [HttpPost("CreateRating")]
        public async Task<IActionResult> Create([FromBody] CreateRatingDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { ratingId = id, message = "Created" });
        }

        [HttpPut("UpdateRating/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRatingDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Updated");
        }

        [HttpDelete("DeleteRating/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok("Deleted");
        }
    }
}
