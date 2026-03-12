using Comman.DTOs.CommanDTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectGroup.Services.UserService;
using UserService.DTOs;
using UserService.Services.Auth;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService,
    IUserService userService,
    IValidator<UserCreateDTO> createValidator
) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
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
        var response = await userService.CreateUser(request);
        return Ok(response);
    }
}
