using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.BusinessObject.Enums;

namespace Ecommerce.BusinessObject.Models;

public class Orders
{
    [Key] public int OrderId { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount  { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.Now;
    
    [ForeignKey(nameof(UserId))] public Users User { get; set; }
    public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    public ICollection<Requests> Requests { get; set; } = new List<Requests>();
}