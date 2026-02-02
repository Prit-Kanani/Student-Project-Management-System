namespace UserService.DTOs;

public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class AuthResponseDTO
{
    public string Message { get; set; }
    public string Token { get; set; }
}

public class UserInfoDTO
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string RoleName { get; set; }
}

