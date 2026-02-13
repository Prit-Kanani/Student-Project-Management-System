using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Services.Auth;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService AuthService
) : ControllerBase
{
    #region Constructor
    private readonly IAuthService AuthService = AuthService;
    #endregion

    #region Login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO request)
    {
        var response = AuthService.Login(request);
        return Ok(response);
    }
    #endregion

}
