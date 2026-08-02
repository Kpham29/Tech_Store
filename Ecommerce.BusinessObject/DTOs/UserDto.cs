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