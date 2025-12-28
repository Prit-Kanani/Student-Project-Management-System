namespace ProjectGroupService.DTOs;

public class GroupWiseStudentListDTO
{
    public int StudentWiseGroupID { get; set; }
    public int ProjectGroupID { get; set; }
    public bool IsActive { get; set; } 
}

public class GroupWiseStudentViewDTO
{
    public int ProjectGroupID { get; set; }
    public int StudentID { get; set; }
    public bool IsActive { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Modified { get; set; }
    public int CreatedByID { get; set; }
    public int? ModifiedByID { get; set; }
}

public class GroupWiseStudentCreateDTO
{
    public int ProjectGroupID { get; set; }
    public int StudentID { get; set; }
    public bool IsActive { get; set; }
    public int CreatedByID { get; set; }
    public int? ModifiedByID { get; set; }
}

public class GroupWiseStudentUpdateDTO
{
    public int StudentWiseGroupID { get; set; }
    public int ProjectGroupID { get; set; }
    public int StudentID { get; set; }
    public bool IsActive { get; set; }
    public int CreatedByID { get; set; }
    public int? ModifiedByID { get; set; }
}