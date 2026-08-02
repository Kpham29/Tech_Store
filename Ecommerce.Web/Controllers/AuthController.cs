using Ecommerce.Web.Helper;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiHelperService _api;

        public AuthController(IApiHelperService api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var result = await _api.PostAsync<LoginResponseModel, LoginViewModel>("api/auth/login", model);

            if (result != null && !string.IsNullOrEmpty(result.Token))
            {
                Response.Cookies.Append("jwtToken", result.Token, new CookieOptions 
                { 
                    HttpOnly = true, 
                    Secure = true, 
                    Expires = DateTime.UtcNow.AddDays(1)
                });
                
                Response.Cookies.Append("username", result.Username);
                Response.Cookies.Append("role", result.Role);
                if (!string.IsNullOrEmpty(result.Avatar)) 
                {
                    Response.Cookies.Append("avatar", result.Avatar);
                }
                
                return RedirectToAction("Index", "Home");
            }
            
            ViewBag.Error = "Username or password is incorrect";
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwtToken");
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("role");
            Response.Cookies.Delete("avatar");
            return RedirectToAction("Index", "Home");
        }
    }
}
