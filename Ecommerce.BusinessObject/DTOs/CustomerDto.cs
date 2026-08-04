using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BusinessObject.DTOs;

public class CustomerDto
{
    public string? Username  { get; set; }
    [Required] [EmailAddress] public string Email { get; set; } = null!;
    public string? Phone  { get; set; }
    public string? Address { get; set; }
    public string? Avatar  { get; set; }
    public DateTime? CreatedAt { get; set; }
}