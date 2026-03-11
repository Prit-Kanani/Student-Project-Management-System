namespace UserService.DTOs;

public class LoginDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
public class AuthResponseDTO
{
    public string? Message { get; set; }
    public string? Token { get; set; }
}

public class UserInfoDTO
{
    public int UserID { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}

