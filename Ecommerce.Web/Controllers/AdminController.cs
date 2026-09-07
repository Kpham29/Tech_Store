using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers;

public class AdminController : Controller
{
    [HttpGet]
    public IActionResult Dashboard()
    {
        var role = HttpContext.Request.Cookies["role"];
        if (role != "Admin") return RedirectToAction("Login", "Auth");
        
        return View();
    }
}
