using E_CommerceDataBusiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteServices _favoriteServices;
        public FavoritesController(IFavoriteServices favoriteServices)
        {
            _favoriteServices = favoriteServices;
        }

        [HttpGet("GetAllFavorite")]
        public async Task<IActionResult> GetUserFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var favorites = await _favoriteServices.GetUserFavorites(userId);
            if (favorites == null) return Ok("No Product found");
            return Ok(favorites);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFavorite([FromRoute] int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {

                await _favoriteServices.RemoveFromFavorite(userId, productId);
                return Ok(new { message = "Product removed from favorites." });
            }
            catch (Exception)
            {

                return NotFound(new { message = "Favorite not found." });
            }
        }
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddFavorite([FromRoute] int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _favoriteServices.AddToFavorite(userId, productId);
            return Ok("Product added to favorites.");
        }

        }
    }
