using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectGroupService.Models;
public class ProjectGroupByProject
{
    [Key]
    public int ProjectGroupByProjectID { get; set; }
    public bool IsActive { get; set; } = true;
    public int ProjectGroupID { get; set; }
    public int ProjectID { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Modified { get; set; }
    public int CreatedByID { get; set; }
    public int? ModifiedByID { get; set; } 

    [ForeignKey(nameof(ProjectGroupID))]
    public ProjectGroup projectGroup { get; set; }

}
