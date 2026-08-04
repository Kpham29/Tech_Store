using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers;

public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult Index(int statusCode)
    {
        switch (statusCode)
        {
            case 401: return View("401");
            case 403: return View("403");
            case 404: return View("404");
            case 500: return View("500");
            default: return View("500");
        }
    }
}
