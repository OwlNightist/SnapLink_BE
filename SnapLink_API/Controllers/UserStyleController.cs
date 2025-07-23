using Microsoft.AspNetCore.Mvc;
using SnapLink_Model.DTO.Request;
using SnapLink_Service.IService;

namespace SnapLink_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserStyleController : ControllerBase
    {
        private readonly IUserStyleService _userStyleService;

        public UserStyleController(IUserStyleService userStyleService)
        {
            _userStyleService = userStyleService;
        }

        /// <summary>
        /// Get user's favorite styles
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserFavoriteStyles(int userId)
        {
            try
            {
                var favoriteStyles = await _userStyleService.GetUserFavoriteStylesAsync(userId);
                return Ok(favoriteStyles);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Add a style to user's favorites
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddUserStyle([FromBody] AddUserStyleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userStyle = await _userStyleService.AddUserStyleAsync(request);
                return CreatedAtAction(nameof(GetUserFavoriteStyles), new { userId = userStyle.UserId }, userStyle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update user's favorite styles (replace all)
        /// </summary>
        [HttpPut("user/{userId}")]
        public async Task<IActionResult> UpdateUserStyles(int userId, [FromBody] List<int> styleIds)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var request = new UpdateUserStylesRequest
                {
                    UserId = userId,
                    StyleIds = styleIds
                };

                var favoriteStyles = await _userStyleService.UpdateUserStylesAsync(request);
                return Ok(favoriteStyles);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove a style from user's favorites
        /// </summary>
        [HttpDelete("user/{userId}/style/{styleId}")]
        public async Task<IActionResult> RemoveUserStyle(int userId, int styleId)
        {
            try
            {
                var result = await _userStyleService.RemoveUserStyleAsync(userId, styleId);
                if (!result)
                {
                    return NotFound(new { message = $"Style {styleId} not found in user {userId}'s favorites" });
                }
                return Ok(new { message = "Style removed from favorites successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get style recommendations based on user's favorites
        /// </summary>
        [HttpGet("user/{userId}/recommendations")]
        public async Task<IActionResult> GetStyleRecommendations(int userId, [FromQuery] int count = 10)
        {
            try
            {
                var recommendations = await _userStyleService.GetStyleRecommendationsAsync(userId, count);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get photographers recommended based on user's favorite styles
        /// </summary>
        [HttpGet("user/{userId}/photographers")]
        public async Task<IActionResult> GetRecommendedPhotographers(int userId, [FromQuery] int count = 10)
        {
            try
            {
                var photographers = await _userStyleService.GetRecommendedPhotographersAsync(userId, count);
                return Ok(photographers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if user has a specific style as favorite
        /// </summary>
        [HttpGet("user/{userId}/style/{styleId}/check")]
        public async Task<IActionResult> CheckUserStyleFavorite(int userId, int styleId)
        {
            try
            {
                var isFavorite = await _userStyleService.IsUserStyleFavoriteAsync(userId, styleId);
                return Ok(new { isFavorite });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get users who have a specific style as favorite
        /// </summary>
        [HttpGet("style/{styleId}/users")]
        public async Task<IActionResult> GetUsersByStyle(int styleId)
        {
            try
            {
                var userIds = await _userStyleService.GetUsersByStyleAsync(styleId);
                return Ok(userIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
} 