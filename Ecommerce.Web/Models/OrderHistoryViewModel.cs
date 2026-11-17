namespace Ecommerce.Web.Models;

public class OrderHistoryViewModel
{
    public int OrderId { get; set; }
    public decimal TotalAmount  { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}