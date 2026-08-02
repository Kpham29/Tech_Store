namespace Ecommerce.Web.Models;

public class LoginViewModel
{
    public string Username  { get; set; }
    public string Password { get; set; }
}

public class LoginResponseModel
{
    public string Username  { get; set; }
    public string Role   { get; set; }
    public string Avatar { get; set; }
    public string Token  { get; set; }
}