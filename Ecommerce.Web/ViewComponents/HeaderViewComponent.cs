using Ecommerce.Web.Helper;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    private readonly IApiHelperService _api;
    
    public HeaderViewComponent(IApiHelperService api)
    {
        _api = api;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cates = await _api.GetAsync<List<CategoryMenuViewModel>>("api/category/menu");
        if (cates == null)
        {
            cates = new List<CategoryMenuViewModel>();
        }

        var token = HttpContext.Request.Cookies["jwtToken"];
        bool isLoggedIn = false;
        string username = HttpContext.Request.Cookies["username"];
        string role = HttpContext.Request.Cookies["role"];

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var profile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");
                if (profile != null)
                {
                    isLoggedIn = true;
                    if (!string.IsNullOrEmpty(profile.Avatar))
                    {
                        ViewBag.Avatar = profile.Avatar;
                    }
                }
                else
                {
                    // Token expired or invalid
                    HttpContext.Response.Cookies.Delete("jwtToken");
                    HttpContext.Response.Cookies.Delete("username");
                    HttpContext.Response.Cookies.Delete("role");
                    username = null;
                    role = null;
                }
            }
            catch
            {
                // Error reading profile, treat as logged out
                HttpContext.Response.Cookies.Delete("jwtToken");
                HttpContext.Response.Cookies.Delete("username");
                HttpContext.Response.Cookies.Delete("role");
                username = null;
                role = null;
            }
        }
        else if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(role))
        {
            // Missing token but still have other cookies, clear them
            HttpContext.Response.Cookies.Delete("username");
            HttpContext.Response.Cookies.Delete("role");
            username = null;
            role = null;
        }

        ViewBag.IsLoggedIn = isLoggedIn;
        ViewBag.Username = username;
        ViewBag.Role = role;

        return View(cates);
    }
}