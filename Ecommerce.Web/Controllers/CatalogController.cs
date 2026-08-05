using Ecommerce.BusinessObject.Helpers;
using Ecommerce.Web.Helper;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers;

public class CatalogController : Controller
{
    private readonly IApiHelperService _api;
    
    public  CatalogController(IApiHelperService api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index([FromQuery] int? cateId, [FromQuery] List<int> brandId, [FromQuery] string? price, 
        [FromQuery] string? sort, [FromQuery] bool? isNew, [FromQuery] string? keyword, [FromQuery] int page = 1)
    {
        var vm = new CatalogViewModel
        {
            SelectedCategoryId = cateId,
            SelectedBrandIds = brandId,
            SelectedPrice = price,
            SelectedSort = sort,
            IsNew = isNew,
            Keyword = keyword
        };

        decimal? minPrice = null;
        decimal? maxPrice = null;
        if (price == "1") maxPrice = 5000000;
        else if (price == "2") { minPrice = 5000000; maxPrice = 15000000; }
        else if (price == "3") { minPrice = 15000000; maxPrice = 25000000; }
        else if (price == "4") minPrice = 25000000;

        int? singleBrandId = brandId != null && brandId.Any() ? brandId.First() : null;
        var productUrl = $"api/products/filter?Status=1&PageIndex={page}&PageSize=15" +
                         (cateId.HasValue ? $"&CategoryId={cateId}" : "") +
                         (singleBrandId.HasValue ? $"&BrandId={singleBrandId}" : "") +
                         (minPrice.HasValue ? $"&MinPrice={minPrice}" : "") +
                         (maxPrice.HasValue ? $"&MaxPrice={maxPrice}" : "") +
                         (isNew.HasValue && isNew.Value ? "&IsNew=true" : "") +
                         (!string.IsNullOrEmpty(keyword) ? $"&Keyword={keyword}" : "") +
                         (!string.IsNullOrEmpty(sort) ? $"&SortBy={sort}" : "");

        var pros = await _api.GetAsync<PagedResult<ProductViewModel>>(productUrl);
        if (pros != null)
            vm.Products = pros;

        var cates = await _api.GetAsync<List<CategoryMenuViewModel>>("api/category/menu");
        if (cates != null)
        {
            vm.Categories = cates;
            vm.Brands = cates.SelectMany(c => c.Brands).DistinctBy(b => b.BrandId).ToList();
        }
        return View(vm);
    }

    public async Task<IActionResult> SearchSuggestions(string keyword)
    {
        var pros = await _api.GetAsync<PagedResult<ProductViewModel>>(
            $"api/products/filter?Status=1&Keyword={keyword}&PageSize=5");
        return Json(pros?.Items ?? new List<ProductViewModel>());
    }

    public async Task<IActionResult> ProductDetail(int id)
    {
        var rs = await _api.GetAsync<ProductDetailViewModel>($"api/products/{id}");
        if (rs == null)
            return RedirectToAction("Index", "Home");
        var related = await _api.GetAsync<List<ProductViewModel>>($"api/products/related-product/{id}");
        if (related != null)
        {
            rs.RelatedProducts = related;
        }
        return View(rs);
    }
    
}