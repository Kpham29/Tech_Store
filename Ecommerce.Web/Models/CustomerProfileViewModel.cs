namespace Ecommerce.Web.Models;

public class CustomerProfileViewModel
{
    public string? Username  { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone  { get; set; }
    public string? Address { get; set; }
    public string? Avatar  { get; set; }
    public DateTime? CreatedAt { get; set; }
}