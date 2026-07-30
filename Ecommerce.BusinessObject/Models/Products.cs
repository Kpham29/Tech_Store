using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.BusinessObject.Enums;

namespace Ecommerce.BusinessObject.Models;

public class Products
{
    [Key] public int ProductId { get; set; }
    [Required] public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public ProductStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.Now;
    
    [ForeignKey(nameof(BrandId))]
    public Brands Brand { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public Categories Category { get; set; }

    public ICollection<ProductImages> ProductImages { get; set; } = new List<ProductImages>();
    public ICollection<ProductAttributes> ProductAttributes { get; set; } = new List<ProductAttributes>();
    public ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
    public ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();
    public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
}