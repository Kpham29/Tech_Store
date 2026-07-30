using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BusinessObject.Models;

public class Brands
{
    [Key]
    public int BrandId { get; set; }
    public string BrandName { get; set; }

    public ICollection<Products> Products = new List<Products>();
    public ICollection<CategoryBrands> CategoryBrands = new List<CategoryBrands>();
}