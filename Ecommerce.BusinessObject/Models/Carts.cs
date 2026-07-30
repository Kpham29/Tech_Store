using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BusinessObject.Models;

public class Carts
{
    [Key] public int CartId { get; set; }
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))] public Users User { get; set; } = null!;
    public ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();
}