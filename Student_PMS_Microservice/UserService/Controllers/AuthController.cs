using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Services.Auth;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService
) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO request)
    {
        var response = await authService.Login(request);
        return Ok(response);
    }
}
