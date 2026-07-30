using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BusinessObject.Models;

public class Categories
{
    [Key]
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }

    public ICollection<Products> Products { get; set; } = new List<Products>();
    public ICollection<CategoryBrands> CategoryBrands = new List<CategoryBrands>();
}