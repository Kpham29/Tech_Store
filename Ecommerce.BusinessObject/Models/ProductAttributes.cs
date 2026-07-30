using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.BusinessObject.Models;

public class ProductAttributes
{
    [Key] public int ProductAttributeId { get; set; }
    public int ProductId { get; set; }
    public int AttributeId { get; set; }
    public string AttributeValue { get; set; } = string.Empty;

    [ForeignKey(nameof(AttributeId))] public Attributes Attribute { get; set; } = null!;

    [ForeignKey(nameof(ProductId))] public Products Product { get; set; } = null!;
}