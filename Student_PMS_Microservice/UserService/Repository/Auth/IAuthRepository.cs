using UserService.DTOs;

namespace UserService.Repository.Auth;

public interface IAuthRepository
{
    Task<LoginDTO> Login(string Email);
    Task<UserInfoDTO> UserInfo(string Email);
}
