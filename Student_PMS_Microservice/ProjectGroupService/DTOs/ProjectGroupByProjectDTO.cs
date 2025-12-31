namespace ProjectGroupService.DTOs;

public class ProjectGroupByProjectListDTO
{
    public int ProjectGroupByProjectID { get; set; }
    public string ProjectGroupName { get; set; }
    public int ProjectID { get; set; }
    public bool IsActive { get; set; }
}
public class ProjectGroupByProjectViewDTO 
{
    public string ProjectGroupName { get; set; }
    public int ProjectID { get; set; }
    public bool IsActive { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? Modified { get; set; }
    public int CreatedByID { get; set; }
    public int? ModifiedByID { get; set; }
}

public class ProjectGroupByProjectCreateDTO
{
    public int ProjectGroupID { get; set; }
    public int ProjectID { get; set; }
    public bool IsActive { get; set; } = true;
    public int? CreatedByID { get; set; }
}

public class BulkProjectGroupByProjectCreateDTO
{
    public int ProjectGroupID { get; set; }
    public int ProjectID { get; set; }
    public bool IsActive { get; set; } = true;
    public int? CreatedByID { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}
public class ProjectGroupByProjectUpdateDTO
{
    public int ProjectGroupByProjectID { get; set; }
    public bool IsActive { get; set; } = true;
    public int ProjectGroupID { get; set; }
    public int ProjectID { get; set; }
    public int? ModifiedByID { get; set; }
}