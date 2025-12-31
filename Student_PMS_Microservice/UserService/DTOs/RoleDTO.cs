namespace UserService.DTOs;

public class RoleListDTO
{
    public int RoleID { get; set; }
    public string RoleName { get; set; }
    public bool IsActive { get; set; }
}
public class RoleViewDTO
{
    public required string RoleName { get; set; }
    public string? Description { get; set; }
    public string CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsActive { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
}


public class RoleCreateDTO
{
    public string RoleName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int CreatedByID { get; set; }
}
public class RoleUpdateDTO
{
    public int RoleID { get; set; }
    public string RoleName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
