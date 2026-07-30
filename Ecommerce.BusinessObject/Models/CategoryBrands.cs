using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BusinessObject.Models;

public class CategoryBrands
{
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    
    [ForeignKey(nameof(CategoryId))]
    public Categories Category { get; set; }
    [ForeignKey(nameof(BrandId))]
    public Brands Brand { get; set; }
}