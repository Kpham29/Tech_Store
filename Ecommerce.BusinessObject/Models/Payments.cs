using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.BusinessObject.Enums;

namespace Ecommerce.BusinessObject.Models;

public class Payments
{
    [Key] public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string Method { get; set; }
    public string? TransactionId  { get; set; }
    public PaymentStatus  Status { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.Now;
    [ForeignKey(nameof(OrderId))] 
    public Orders Order { get; set; }
}