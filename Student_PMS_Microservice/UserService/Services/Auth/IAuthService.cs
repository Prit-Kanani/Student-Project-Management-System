using UserService.DTOs;

namespace UserService.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDTO> Login(LoginDTO login);
}

