using E_CommerceDataBusiness.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (userId == null) return Unauthorized();

            var favorites = await _favoriteServices.GetUserFavorites("27c2d1ae-0769-4d72-b3a0-f888668ed807");
            if (favorites == null) return Ok("No Product found");
            return Ok(favorites);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFavorite([FromRoute] int productId)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (userId == null) return Unauthorized();

            try
            {

                await _favoriteServices.RemoveFromFavorite("27c2d1ae-0769-4d72-b3a0-f888668ed807", productId);
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
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (userId == null) return Unauthorized();
            
            await _favoriteServices.AddToFavorite("27c2d1ae-0769-4d72-b3a0-f888668ed807", productId);
            return Ok("Product added to favorites.");
        }

        }
    }
