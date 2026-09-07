using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.ViewComponents;

public class AdminSidebarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var role = HttpContext.Request.Cookies["role"];
        ViewBag.Role = role ?? "";
        return View();
    }
}
