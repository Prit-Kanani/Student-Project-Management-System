using Comman.Functions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;

public class User
{
    [Key]
    [OptionId]
    public int UserID { get; set; }

    [OptionName]
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Modified { get; set; }
    public int? RoleID { get; set; }
    public int? CreatedByID { get; set; }
    public int? ModifiedByID { get; set; }

    [ForeignKey(nameof(CreatedByID))]
    public User? CreatedBy { get; set; }

    [ForeignKey(nameof(ModifiedByID))]
    public User? ModifiedBy { get; set; }

    [ForeignKey(nameof(RoleID))]
    public Role? Role { get; set; }
}
