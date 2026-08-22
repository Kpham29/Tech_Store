using System.Security.Claims;
using Ecommerce.API.Helpers;
using Ecommerce.BusinessObject;
using Ecommerce.BusinessObject.DTOs;
using Ecommerce.BusinessObject.Enums;
using Ecommerce.BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly HttpClient  _httpClient;
        
        public OrderController(EcommerceDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }
        
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            decimal totalAmount = 0;
            foreach (var i in dto.Items)
            {
                var pro = await _context.Products.FindAsync(i.ProductId);
                totalAmount += pro.Price *  i.Quantity;
            }

            var order = new Orders
            {
                UserId = userId,
                TotalAmount = totalAmount,
                ShippingAddress = dto.ShippingAddress,
                ShippingPhone =  dto.ShippingPhone,
                Status = OrderStatus.Pending
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

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
                await  _context.SaveChangesAsync();
            }

            var payment = new Payments
            {
                OrderId = order.OrderId,
                Method =  dto.PaymentMethod,
                TransactionId = "TG" + RandomTransaction.GenerateRandomTransaction(13),
                Status = PaymentStatus.Pending
            };
            _context.Payments.Add(payment);
            
            await  _context.SaveChangesAsync();

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
            
            return Ok(new {success = true, orderId = order.OrderId});
        }
    }
}
