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
        return View(cates);
    }
}