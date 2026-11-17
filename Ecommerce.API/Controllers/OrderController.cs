using System.Security.Claims;
using Ecommerce.API.Helpers;
using Ecommerce.BusinessObject;
using Ecommerce.BusinessObject.DTOs;
using Ecommerce.BusinessObject.Enums;
using Ecommerce.BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Controllers
{
    public class SePayResponse
    {
        public List<SePayTransaction> transactions { get; set; }
    }
    public class SePayTransaction
    {
        public decimal amount_in { get; set; }
        public string transaction_content { get; set; }
    }

    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly HttpClient _httpClient;

        public OrderController(EcommerceDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            decimal totalAmount = 0;
            foreach (var i in dto.Items)
            {
                var pro = await _context.Products.FindAsync(i.ProductId);
                totalAmount += pro.Price * i.Quantity;
            }

            var order = new Orders
            {
                UserId = userId,
                TotalAmount = totalAmount,
                ShippingAddress = dto.ShippingAddress,
                ShippingPhone = dto.ShippingPhone,
                Status = OrderStatus.Pending
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

            foreach (var i in dto.Items)
            {
                var pro = await _context.Products.FindAsync(i.ProductId);
                var newItem = new OrderDetails
                {
                    OrderId = order.OrderId,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = pro.Price,
                };
                _context.OrderDetails.Add(newItem);

                if (cart != null)
                {
                    var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == i.ProductId);
                    if (cartItem != null)
                    {
                        _context.CartItems.Remove(cartItem);
                    }
                }

                await _context.SaveChangesAsync();
            }

            var payment = new Payments
            {
                OrderId = order.OrderId,
                Method = dto.PaymentMethod,
                TransactionId = "TG" + RandomTransaction.GenerateRandomTransaction(13),
                Status = PaymentStatus.Pending
            };
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            if (dto.PaymentMethod.Contains("Online", StringComparison.OrdinalIgnoreCase))
            {
                string url = GetQrUrl.GetUrl(order.TotalAmount, payment.TransactionId);
                return Ok(new
                {
                    success = true,
                    orderId = order.OrderId,
                    paymentId = payment.PaymentId,
                    transactionId = payment.TransactionId,
                    totalAmount = order.TotalAmount,
                    qrUrl = url
                });
            }

            return Ok(new { success = true, orderId = order.OrderId });
        }

        [HttpGet("customer-history")]
        public async Task<IActionResult> GetCustomerOrderHistory([FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            var query = _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt);
            var selectedQuery = query.Select(o => new OrderHistoryDto
            {
                OrderId = o.OrderId,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CreatedAt = o.CreatedAt
            });
            
            var data = await selectedQuery.ToPagedResultAsync(pageIndex, pageSize);
            return Ok(data);
        }
    }
}
