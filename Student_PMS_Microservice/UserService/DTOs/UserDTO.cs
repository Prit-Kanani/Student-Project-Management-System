namespace UserService.DTOs.UserDTO;

public class UserListDTO
{
    public int UserID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsActive { get; set; }
}

public class UserViewDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string? RoleName { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsActive { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }

}

public class UserCreateDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public int? RoleID { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UserUpdateDTO
{
    public int UserID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int? RoleID { get; set; }
    public bool IsActive { get; set; } = true;
}
