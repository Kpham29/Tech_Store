using Ecommerce.BusinessObject.Enums;

namespace Ecommerce.Web.Models;

public class RegisterViewModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Phone  { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class RegisterResponseModel
{
    public string Message  { get; set; }
}