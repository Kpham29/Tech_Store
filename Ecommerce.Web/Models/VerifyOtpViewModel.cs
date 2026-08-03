namespace Ecommerce.Web.Models;

public class VerifyOtpViewModel
{
    public string Email { get; set; } = null!;
    public string Otp { get; set; } = null!;
}

public class SendOtpViewModel
{
    public string Email { get; set; } = null!;
}