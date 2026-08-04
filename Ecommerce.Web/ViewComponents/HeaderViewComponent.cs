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
        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var profile = await _api.GetAsync<CustomerProfileViewModel>("api/customer/profile");
                if (profile != null && !string.IsNullOrEmpty(profile.Avatar))
                {
                    ViewBag.Avatar = profile.Avatar;
                }
            }
            catch
            {
                // Ignore errors to ensure the header always renders
            }
        }

        return View(cates);
    }
}