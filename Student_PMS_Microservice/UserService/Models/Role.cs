using Comman.Functions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectGroup.Models;

public class Role
{
    [Key]
    [OptionId]
    public int RoleID { get; set; }

    [OptionName]
    public string RoleName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Modified { get; set; }
    public int CreatedByID { get; set; }
    public int? ModifiedByID { get; set; }

    [ForeignKey(nameof(CreatedByID))]
    public User CreatedBy { get; set; }

    [ForeignKey(nameof(ModifiedByID))]
    public User? ModifiedBy { get; set; }
}
