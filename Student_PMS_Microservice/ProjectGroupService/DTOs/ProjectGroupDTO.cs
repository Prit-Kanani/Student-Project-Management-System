namespace ProjectGroupService.DTOs;

public class ProjectGroupListDTO
{
    public int ProjectGroupID { get; set; }
    public string ProjectGroupName { get; set; } = string.Empty;
    public bool? IsApproved { get; set; }
    public bool IsActive { get; set; }
}

public class ProjectGroupViewDTO
{
    public string ProjectGroupName { get; set; } = string.Empty;
    public string ApprovalStatusString { get; set; } = string.Empty;
    public string? ApprovedBy { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsActive { get; set; } 
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }

}

public class ProjectGroupCreateDTO
{
    public string ProjectGroupName { get; set; } = string.Empty;
    public bool? IsApproved { get; set; } = null;
    public bool IsActive { get; set; } = true;
    public int? ApprovedByID { get; set; } = null;
    public int CreatedByID { get; set; }
}

public class ProjectGroupUpdateDTO
{
    public int ProjectGroupID { get; set; }
    public string ProjectGroupName { get; set; } = string.Empty;
    public bool? IsApproved { get; set; }
    public int? ApprovedByID { get; set; }
    public bool IsActive { get; set; }
    public int? ModifiedByID { get; set; }
}
