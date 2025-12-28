namespace ProjectService.DTOs;

public class ProjectListDTO
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; }
    public bool? IsApproved { get; set; }
    public bool IsActive { get; set; }
    public bool IsCompleted { get; set; }
}

public class ProjectViewDTO
{
    public string ProjectName { get; set; }
    public bool? IsApproved { get; set; }
    public string? Description { get; set; }
    public int CreatedByID { get; set; }
    public int? ModifiedByID { get; set; }
    public bool IsActive { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
}

public class ProjectCreateDTO
{
    public string ProjectName { get; set; }
    public string? Description { get; set; }
    public bool? IsApproved { get; set; }
    public bool IsActive { get; set; }
    public bool IsCompleted { get; set; }
    public int CreatedByID { get; set; }
}

public class ProjectUpdateDTO
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; }
    public string? Description { get; set; }
    public bool? IsApproved { get; set; }
    public bool IsActive { get; set; }
    public bool IsCompleted { get; set; }
    public int ModifiedByID { get; set; }
}
