using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BusinessObject.Models;

public class OrderDetails
{
    [Key] public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity  { get; set; }
    public decimal UnitPrice { get; set; }
    [ForeignKey(nameof(OrderId))] public Orders Order { get; set; }
    [ForeignKey(nameof(ProductId))] public Products Product { get; set; }
}