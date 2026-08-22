using Ecommerce.BusinessObject;
using Ecommerce.BusinessObject.Enums;
using Ecommerce.BusinessObject.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        
        public PaymentController(EcommerceDbContext context, IConfiguration config, HttpClient client)
        {
            _context = context;
            _config = config;
            _client = client;
        }

        [HttpGet("check-payment/{paymentId}")]
        public async Task<IActionResult> CheckPayment(int paymentId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if(payment == null) return NotFound();
            var order = await _context.Orders.FindAsync(payment.OrderId);
            if (payment.Status == PaymentStatus.Completed)
            {
                return Ok(new { success = true });
            }
            string token = _config["SePay:API"];
            string apiUrl = "https://userapi-sandbox.sepay.vn/transactions/list?limit=10";
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("Authorization", $"Bearer {token}");
            
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<SePayResponse>(content);

                if (data != null && data.transactions != null)
                {
                    var validTransaction = data.transactions.FirstOrDefault(t =>
                        t.transaction_content.Contains(payment.TransactionId, StringComparison.OrdinalIgnoreCase)
                        && t.amount_in == order.TotalAmount);
                    if (validTransaction != null)
                    {
                        payment.Status = PaymentStatus.Completed;
                        await _context.SaveChangesAsync();
                        return Ok(new { success = true });
                    }
                }
            }
            return Ok(new { status = "PENDING" });
        }
    }
}
