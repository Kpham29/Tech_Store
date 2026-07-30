using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BusinessObject.Models;

public class Attributes
{
    [Key]
    public int AttributeId { get; set; }
    public string AttributeName { get; set; }

    public ICollection<ProductAttributes> ProductAttributes = new List<ProductAttributes>();
}