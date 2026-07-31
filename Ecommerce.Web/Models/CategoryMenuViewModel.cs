namespace Ecommerce.Web.Models;

public class CategoryMenuViewModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public List<BrandMenuViewModel> Brands { get; set; } = new List<BrandMenuViewModel>();
}

public class BrandMenuViewModel
{
    public int BrandId { get; set; }
    public string BrandName { get; set; }
}