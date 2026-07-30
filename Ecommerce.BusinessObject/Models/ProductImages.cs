using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BusinessObject.Models;

public class ProductImages
{
    [Key] public int ProductImageId { get; set; }
    public int ProductId { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsThumbnail { get; set; }
    [ForeignKey(nameof(ProductId))] public Products Product { get; set; } = null!;

}