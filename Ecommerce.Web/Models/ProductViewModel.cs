using Ecommerce.BusinessObject.Helpers;

namespace Ecommerce.Web.Models;

public class ProductViewModel
{
    public int ProductId  { get; set; }
    public string ProductName  { get; set; }
    public decimal Price   { get; set; }
    public string Thumbnail { get; set; }
}

public class ProductViewModelList
{
    public List<ProductViewModel> NewProducts { get; set; } = new List<ProductViewModel>();
    public List<ProductViewModel> Laptops { get; set; } = new List<ProductViewModel>();
    public List<ProductViewModel> Monitors { get; set; } = new List<ProductViewModel>();
    public List<ProductViewModel> MsiProducts { get; set; } = new List<ProductViewModel>();
    public List<ProductViewModel> Keyboards { get; set; } = new List<ProductViewModel>();
}

public class CatalogViewModel
{
    public PagedResult<ProductViewModel> Products { get; set; } = new PagedResult<ProductViewModel>();
    public List<CategoryMenuViewModel> Categories { get; set; } = new List<CategoryMenuViewModel>();

    public List<BrandMenuViewModel> Brands { get; set; } = new List<BrandMenuViewModel>();
    
    public int? SelectedCategoryId { get; set; }
    public List<int> SelectedBrandIds { get; set; } = new List<int>();
    public string? SelectedPrice { get; set; }
    public string? SelectedSort  { get; set; }
    public bool? IsNew { get; set; }
    public string? Keyword  { get; set; }
}

public class ProductDetailViewModel
{
    public int ProductId  { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity  { get; set; }
    public string? Description  { get; set; }
    public string Thumbnail { get; set; }
    
    public string CategoryName  { get; set; }
    public string BrandName  { get; set; }
    
    public List<string> ProductImages { get; set; } = new List<string>();
    public Dictionary<string, string> Attributes { get; set; } =  new Dictionary<string, string>();
    public List<ProductViewModel> RelatedProducts { get; set; } = new List<ProductViewModel>();
}