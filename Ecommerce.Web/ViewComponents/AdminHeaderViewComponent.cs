using Ecommerce.Web.Helper;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.ViewComponents;

public class AdminHeaderViewComponent : ViewComponent
{
    private readonly IApiHelperService _api;
    
    public AdminHeaderViewComponent(IApiHelperService api)
    {
        _api = api;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var token = HttpContext.Request.Cookies["jwtToken"];
        string username = HttpContext.Request.Cookies["username"];
        string role = HttpContext.Request.Cookies["role"];
        string avatar = null;

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var profile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");
                if (profile != null)
                {
                    avatar = profile.Avatar;
                }
            }
            catch
            {
                // Ignore error, avatar remains null
            }
        }

        ViewBag.Username = username;
        ViewBag.Role = role;
        ViewBag.Avatar = avatar;

        return View();
    }
}
