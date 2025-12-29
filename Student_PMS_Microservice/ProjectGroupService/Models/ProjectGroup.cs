using Comman.Functions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectGroupService.Models;
public class ProjectGroup
{
    [Key]
    [OptionId]
    public int ProjectGroupID { get; set; }

    [OptionName]
    public string ProjectGroupName { get; set; }
    public bool? IsApproved { get; set; } 
    public bool IsActive { get; set; } = true;
    public int? ApprovedByID { get; set; }
    public int CreatedByID { get; set; }
    public int? ModifiedByID { get; set; } 
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Modified { get; set; }

    [NotMapped]
    public string ApprovalStatusString =>
        IsApproved switch
        {
            true => "Approved",
            false => "Rejected",
            null => "Pending"
        };
}
