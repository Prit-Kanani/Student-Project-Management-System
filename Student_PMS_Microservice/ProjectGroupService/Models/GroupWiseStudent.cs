using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectGroupServices.Models;

public class GroupWiseStudent
{
    [Key]
    public int StudentWiseGroupID { get; set; }
    public bool IsActive { get; set; } = true;
    public int ProjectGroupID { get; set; }
    public int StudentID { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Modified { get; set; }
    public int CreatedByID { get; set; }
    public int? ModifiedByID { get; set; }

    [ForeignKey(nameof(ProjectGroupID))]
    public ProjectGroup ProjectGroup { get; set; }
}
