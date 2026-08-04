using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BusinessObject.DTOs;

public class LoginDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class RegisterDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string Phone  { get; set; }
    public string? Address  { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class SendOtpDto
{
    [Required] [EmailAddress] public string Email { get; set; } = null!;
}

public class VerifyOtpDto
{
    [Required] [EmailAddress] public string Email { get; set; } = null!;
    [Required] public string Otp { get; set; } = null!;
}

public class ResetPasswordDto
{
    public string Email { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}

