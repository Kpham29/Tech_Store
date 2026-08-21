namespace Ecommerce.Web.Models;

public class CustomerProfileViewModel
{
    public string? Username  { get; set; }
    public string? FullName  { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone  { get; set; }
    public string? Address { get; set; }
    public string? Avatar  { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class ChangePasswordViewModel
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}