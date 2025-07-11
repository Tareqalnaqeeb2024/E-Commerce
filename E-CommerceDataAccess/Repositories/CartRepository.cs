using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Repositories
{
    public class CartRepository : ICartRepository
        
    {
        private readonly AppDbContext  _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddToCartAsync(CreateCartItemDTO cartItemDTO, string userId)
        {



            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserID == userId) ?? new Cart { UserID = userId };




            if (cart.CartId == 0)
            {
               
               await  _context.Carts.AddAsync(cart);
              
            }

            var existingItem = cart.Items?.FirstOrDefault(i => i.ProductId == cartItemDTO.ProductID);

            if (existingItem != null)
            {
                existingItem.Quantity += cartItemDTO.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = cartItemDTO.ProductID,
                    Quantity = cartItemDTO.Quantity,
                    Cart= cart
                };

                cart.Items ??= new List<CartItem>();
                cart.Items.Add(cartItem);
            }
            await _context.SaveChangesAsync();

        }

        public async Task DecrementItemQuantityAsnyc(string userId, int productId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserID == userId);
            if (cart == null) throw new Exception("Cart not found.");

            var item = await _context.CartItems.FirstOrDefaultAsync(i => i.CartId == cart.CartId && i.ProductId == productId);
            if (item == null) throw new Exception("Item not found in cart.");

            if (item.Quantity > 1)
            {
                item.Quantity--;
            }else
            {
                _context.CartItems.Remove(item);
            }
            await _context.SaveChangesAsync();


        }

        public async Task<CartDTO> GetCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null)

                return new CartDTO
                {
                    UserID = userId,
                    Items = new List<GetCartItemDTO>(),
                    TotalAmount = 0
                };
            var cartDTO = new CartDTO
            {
                UserID = cart.UserID,
                Items = cart.Items.Select(item => new GetCartItemDTO
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                 

                }).ToList(),
                TotalAmount = cart.TotalAmount,

            };

            return cartDTO;
            
        }

        public async Task IncrementItemQuantityAsnyc(int productId, string userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserID == userId);
            if (cart == null) throw new Exception("Cart not found.");

            var item = await _context.CartItems.FirstOrDefaultAsync(i => i.CartId == cart.CartId && i.ProductId == productId);
            if (item == null) throw new Exception("Item not found in cart.");

            item.Quantity++;
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserID == userId);
            if (cart == null) throw new Exception("Cart not found.");

            var item = await _context.CartItems.FirstOrDefaultAsync(i => i.CartId == cart.CartId && i.ProductId == productId);
            if (item == null) throw new Exception("Item not found in cart.");

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
