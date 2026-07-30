using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BusinessObject.Models;

public class CartItems
{
    [Key] public int CartItemId { get; set; }
    public int CartId { get; set; }
    public int ProductId  { get; set; }
    public int Quantity { get; set; }
    [ForeignKey(nameof(ProductId))] public Products Product { get; set; }
    [ForeignKey(nameof(CartId))] public Carts Cart { get; set; }
}