using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.BusinessObject.Enums;

namespace Ecommerce.BusinessObject.Models;

public class Requests
{
    [Key] public int RequestId { get; set; }
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Reason { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.Now;
    [ForeignKey(nameof(UserId))] public Users User { get; set; }
    [ForeignKey(nameof(OrderId))] public Orders Order { get; set; }
    
}