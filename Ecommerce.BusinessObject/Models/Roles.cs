using System.ComponentModel.DataAnnotations;

namespace Ecommerce.BusinessObject.Models;

public class Roles
{
    [Key]
    public int RoleId  { get; set; }
    [Required]
    [StringLength(10)]
    public string RoleName { get; set; } = string.Empty;

    public virtual ICollection<Users> Users { get; set; } = new List<Users>();
}