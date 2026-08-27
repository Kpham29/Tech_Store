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

        [HttpGet("check-payment/{transactionId}")]
        public async Task<IActionResult> CheckPayment(string transactionId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
            if(payment == null) return NotFound();
            var order = await _context.Orders.FindAsync(payment.OrderId);
            if (payment.Status == PaymentStatus.Completed)
            {
                return Ok(new { success = true });
            }
            string token = _config["SePay:API"];
            
            // Standard SePay API for transactions is my.sepay.vn.
            // If the user's token is for the SePay Hub, this should work. 
            // Often, userapi-sandbox.sepay.vn throws 520 if the environment doesn't exist anymore.
            // Let's use the official URL or the one provided. If it's sandbox, let's keep it but handle the error.
            string apiUrl = "https://my.sepay.vn/userapi/transactions/list?limit=20";
            
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("Authorization", $"Bearer {token}");
            
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };
                
                try 
                {
                    var data = System.Text.Json.JsonSerializer.Deserialize<SePayResponse>(content, options);

                    if (data != null && data.transactions != null)
                    {
                        var validTransaction = data.transactions.FirstOrDefault(t =>
                            t.transaction_content.Contains(payment.TransactionId, StringComparison.OrdinalIgnoreCase)
                            && t.amount_in == order.TotalAmount);
                        if (validTransaction != null)
                        {
                            payment.Status = PaymentStatus.Completed;
                            order.Status = OrderStatus.Processing;
                            await _context.SaveChangesAsync();
                            return Ok(new { success = true });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing SePay response: {ex.Message}");
                    Console.WriteLine($"Response content: {content}");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"SePay API failed with Status Code: {response.StatusCode}");
                Console.WriteLine($"SePay API Error Content: {errorContent}");
            }
            return Ok(new { status = "PENDING" });
        }
    }
}
