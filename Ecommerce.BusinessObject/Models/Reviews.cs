using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BusinessObject.Models;

public class Reviews
{
    [Key] public int ReviewId { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.Now;
    [ForeignKey(nameof(UserId))] 
    public Users User { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Products Product { get; set; }
}