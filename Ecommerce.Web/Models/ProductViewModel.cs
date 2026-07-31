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