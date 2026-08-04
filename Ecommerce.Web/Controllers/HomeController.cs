using System.Diagnostics;
using Ecommerce.BusinessObject.Helpers;
using Ecommerce.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Web.Models;

namespace Ecommerce.Web.Controllers;

public class HomeController : Controller
{
    private readonly IApiHelperService _api;
    
    public HomeController(IApiHelperService api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        var taskNews = _api.GetAsync<PagedResult<ProductViewModel>>("api/products/filter?isNew=true&pageSize=6");
        var taskLaptops = _api.GetAsync<PagedResult<ProductViewModel>>("api/products/filter?categoryId=1&pageSize=5");
        var taskMsi = _api.GetAsync<PagedResult<ProductViewModel>>("api/products/filter?brandId=3&pageSize=5");
        var taskMonitor = _api.GetAsync<PagedResult<ProductViewModel>>("api/products/filter?categoryId=2&pageSize=5");
        var taskKeyboards = _api.GetAsync<PagedResult<ProductViewModel>>("api/products/filter?categoryId=4&pageSize=5");

        await Task.WhenAll(taskNews, taskLaptops, taskMsi, taskMonitor, taskKeyboards);

        var rs = new ProductViewModelList
        {
            NewProducts = taskNews.Result?.Items ?? new List<ProductViewModel>(),
            Laptops = taskLaptops.Result?.Items ?? new List<ProductViewModel>(),
            MsiProducts = taskMsi.Result?.Items ?? new List<ProductViewModel>(),
            Monitors = taskMonitor.Result?.Items ?? new List<ProductViewModel>(),
            Keyboards = taskKeyboards.Result?.Items ?? new List<ProductViewModel>()
        };
        return View(rs);
    }

    [Route("Contact")]
    public IActionResult Contact()
    {
        return View();
    }
}