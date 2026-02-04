using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UserService.Services.Auth;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService AuthService
) : ControllerBase
{
    #region Constructor
    private readonly IAuthService _jwtTokenService;
    #endregion

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // TODO: Validate credentials against your database
        // This is a simplified example
        if (request.Email == "user@example.com" && request.Password == "password123")
        {
            var token = _jwtTokenService.GenerateToken(
                userId: "12345",
                email: request.Email,
                roles: new List<string> { "User", "Admin" }
            );

            return Ok(new { Token = token });
        }

        return Unauthorized(new { Message = "Invalid credentials" });
    }
}
