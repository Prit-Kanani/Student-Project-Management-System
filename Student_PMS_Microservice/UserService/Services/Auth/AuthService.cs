using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using Comman.Functions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.DTOs;
using UserService.Repository.Auth;
using UserService.Repository.UserRepository;

namespace UserService.Services.Auth;

public class AuthService(
    IConfiguration configuration,
    IAuthRepository authRepository,
    IUserRepository userRepository
) : IAuthService
{
    private readonly JwtSettings _jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
        ?? throw new InvalidOperationException("JWT configuration is missing.");

    public async Task<AuthResponseDTO> Login(LoginDTO dto)
    {
        var login = await authRepository.Login(dto.Email)
            ?? throw new UnauthorizedException("Invalid credentials");

        var hashPassword = HashPass.HashPassword(dto.Password);
        if (login.Password != hashPassword)
        {
            throw new UnauthorizedException("Invalid credentials");
        }

        var userInfo = await authRepository.UserInfo(dto.Email)
            ?? throw new UnauthorizedException("Invalid credentials");

        return new AuthResponseDTO
        {
            Message = "Login successful",
            Token = GenerateToken(userInfo)
        };
    }

    public async Task<OperationResultDTO> Register(UserCreateDTO dto)
    {
        return await userRepository.CreateUser(dto);
    }

    private string GenerateToken(UserInfoDTO userInfo)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userInfo.UserID.ToString()),
            new(ClaimTypes.NameIdentifier, userInfo.UserID.ToString()),
            new(ClaimTypes.Name, userInfo.UserName),
            new(JwtRegisteredClaimNames.UniqueName, userInfo.UserName),
            new(JwtRegisteredClaimNames.Email, userInfo.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrWhiteSpace(userInfo.RoleName))
        {
            claims.Add(new Claim(ClaimTypes.Role, userInfo.RoleName));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
