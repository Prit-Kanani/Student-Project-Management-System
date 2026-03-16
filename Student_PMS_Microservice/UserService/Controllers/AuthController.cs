using Comman.DTOs.CommanDTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Services.Auth;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService,
    IValidator<UserCreateDTO> createValidator
) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [Produces<AuthResponseDTO>]
    public async Task<IActionResult> Login([FromBody] LoginDTO request)
    {
        var response = await authService.Login(request);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> Register([FromBody] UserCreateDTO request)
    {
        await createValidator.ValidateAndThrowAsync(request);
        var response = await authService.Register(request);
        return Ok(response);
    }
}
