using Comman.DTOs.CommanDTOs;
using UserService.DTOs;

namespace UserService.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDTO> Login(LoginDTO login);
    Task<OperationResultDTO> Register(UserCreateDTO dto);
}
