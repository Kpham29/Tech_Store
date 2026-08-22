using Ecommerce.BusinessObject;
using Ecommerce.BusinessObject.Enums;
using Ecommerce.BusinessObject.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly IConfiguration _config;
        public PaymentController(EcommerceDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPut("update-status/{paymentId}")]
        public async Task<IActionResult> UpdatePaymentStatus(int paymentId)
        {
            var p = await _context.Payments.FindAsync(paymentId);
            if(p == null) return NotFound();
            if (p.Method == "COD")
            {
                p.Status = PaymentStatus.Completed;
                await _context.SaveChangesAsync();
                return Ok(new { success = true, status = p.Status });
            }

            if (p.Method == "Online")
            {
                var apiToken = _config["ApiToken"];
                if (string.IsNullOrEmpty(apiToken))
                {
                    return StatusCode(500, "SePay API Token is not configured");
                }
                
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");
                int maxAttempt = 6;
                for (int i = 0; i < maxAttempt; i++)
                {
                    try
                    {
                        var resp = await httpClient.GetAsync("https://my.sepay.vn/userapi/transactions/list");
                        if (resp.IsSuccessStatusCode)
                        {
                            var content = await resp.Content.ReadAsStringAsync();
                            var data = System.Text.Json.JsonSerializer.Deserialize<SePayResponse>(content);
                            if (data != null && data.transactions != null)
                            {
                                bool isPaid = data.transactions.Any(t =>
                                    t.transaction_content.Contains(p.TransactionId,
                                        StringComparison.OrdinalIgnoreCase));
                                if (isPaid)
                                {
                                    p.Status = PaymentStatus.Completed;
                                    await _context.SaveChangesAsync();
                                    return Ok(new { success = true, status = p.Status, message = "Online payment successful" });
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    await Task.Delay(5000);
                }

                p.Status = PaymentStatus.Failed;
                await _context.SaveChangesAsync();
                return Ok(new { success = false, status = p.Status, message = "Payment expired" });
            }

            return BadRequest();
        }
    }
}
