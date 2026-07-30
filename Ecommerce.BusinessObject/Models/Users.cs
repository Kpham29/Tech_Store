using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.BusinessObject.Enums;

namespace Ecommerce.BusinessObject.Models;

public class Users
{
    [Key] public int UserId { get; set; }
    [Required] [StringLength(50)]  public string UserName { get; set; } = string.Empty;
    [Required] [StringLength(255)]  public string Password { get; set; } = string.Empty;
    [MaxLength(20)] public string Phone { get; set; } = string.Empty;
    [Required] 
    [EmailAddress]  public string Email { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; }
    public int RoleId { get; set; }
    public string? Address  { get; set; }
    public DateTime CreatedAt  { get; set; } = DateTime.Now;
    
    [ForeignKey(nameof(RoleId))]
    public Roles Role { get; set; }

    public ICollection<Orders> Orders { get; set; } = new List<Orders>();
    public ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
    public ICollection<Requests> Requests { get; set; } = new List<Requests>();
}