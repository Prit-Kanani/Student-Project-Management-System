using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.DTOs;
using UserService.Exceptions;
using UserService.Repository.Auth;

namespace UserService.Services.Auth;

public class AuthService(
    IConfiguration configuration,
    IAuthRepository authRepository
) : IAuthService
{
    #region CONFIGURATION]
    private readonly JwtSettings _jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
    #endregion CONFIGURATION

    #region LOGIN
    public async Task<AuthResponseDTO> Login(LoginDTO dto)
    {
        var login = await authRepository.Login(dto.Email) ?? throw new UnauthorizedException("Invalid credentials");
        var hashPassword = HashPass.HashPassword(dto.Password);
        if ( login.Password != hashPassword)
        {
            throw new UnauthorizedException("Invalid credentials");
        }
        var userInfo =  await UserInfo(dto.Email);
        var token = GenerateToken(userInfo);
        var response = new AuthResponseDTO
        {
            Message = "Login successful",
            Token = token
        };
        return response;
    }
    #endregion
    
    #region GENERATE JWT TOKEN
    private string GenerateToken(UserInfoDTO userInfo)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserID.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.UserName),
            new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, userInfo.RoleName)
        };

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
    #endregion

    #region USER INFO
    private async Task<UserInfoDTO> UserInfo(string Email)
    {
        var response = await authRepository.UserInfo(Email);
        return response;
    }
    #endregion

}
