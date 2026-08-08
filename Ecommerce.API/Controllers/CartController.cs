using System.Security.Claims;
using Ecommerce.BusinessObject;
using Ecommerce.BusinessObject.DTOs;
using Ecommerce.BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Controllers
{
    [Route("api/carts")]
    [ApiController]
    [Authorize(Roles = "3")]
    public class CartController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        
        public CartController(EcommerceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Carts{ UserId = userId};
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var dto = new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CartItems = cart.CartItems?.Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    CartId = ci.CartId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.ProductName,
                    Thumbnail = ci.Product.ProductImages.Where(i => i.IsThumbnail == true).FirstOrDefault().Url,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price,
                }).ToList(),
            };
            return Ok(dto);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCartCount()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);
            int count = cart?.CartItems?.Sum(ci => ci.Quantity) ?? 0;
            return Ok(new { count });
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddCartItemDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Carts { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            var existedItem = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if(existedItem != null)
                existedItem.Quantity += dto.Quantity;
            else
            {
                _context.CartItems.Add(new CartItems
                {
                    CartId = cart.CartId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                });
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cart added successfully" });
        }

        [HttpPut("update-cart")]
        public async Task<IActionResult> UpdateCart([FromBody] UpdateCartDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return NotFound();
            foreach (var item in dto.Items)
            {
                var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci =>
                    ci.CartItemId == item.CartItemId && ci.CartId == cart.CartId);
                if (cartItem != null)
                {
                    cartItem.Quantity = item.Quantity;
                    if (cartItem.Quantity <= 0) _context.CartItems.Remove(cartItem);
                }
            }
            await  _context.SaveChangesAsync();
            return Ok(new { message = "Cart updated successfully" });
        }

        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return NotFound();
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.CartId == cart.CartId);
            
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return Ok(new { message = "Item removed successfully" });
        }
        
    }
}
