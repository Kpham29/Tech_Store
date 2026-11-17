using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers;

public class StaffController : Controller
{
    [HttpGet]
    public IActionResult Dashboard()
    {
        var role = HttpContext.Request.Cookies["role"];
        if (role != "Staff" && role != "Admin") return RedirectToAction("Login", "Auth");
        
        return View();
    }
}
