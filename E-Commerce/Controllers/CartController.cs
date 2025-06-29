using E_CommerceDataAccess.DTO;
using E_CommerceDataBusiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController( ICartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        [HttpGet("GetAllItemsFromCart")]
        public async Task<IActionResult> GetItemsOfCart()
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var cart = await _cartService.GetCartAsync(userId);
            return  cart != null ? Ok(cart) : NotFound("Cart Not Found");
        }

        //[Authorize]
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] CreateCartItemDTO createCartItemDTO )
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }
            await _cartService.AddToCartAsync(createCartItemDTO, userId);
            return Ok(" Added To Cart Successfuly");

        }

        [HttpDelete("remove/{ProductId}")]
        public async Task<IActionResult> RemoveFromCart(int ProductId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            await _cartService.RemoveFromCartAsync(userId, ProductId);
            return Ok("The Product was removed");
        }

        [HttpPost("increment/{productId}")]
        public async Task<IActionResult> IncrementItemQuantity(int productId)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var reslut =  _cartService.IncrementItemQuantityAsnyc(productId, userId);
            return Ok(reslut);
        }


        [HttpPost("decrement/{productId}")]
        public async Task<IActionResult> DecrementItemQuantity(int productId)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var reslut = _cartService.DecrementItemQuantityAsnyc(userId, productId);
            return Ok(reslut);
        }


    }
}
