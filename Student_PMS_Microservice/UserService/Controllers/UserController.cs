using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Comman.MicroserviceDTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProjectGroup.Services.UserService;
using UserService.DTOs;

namespace UserService.Controllers;

[Route("api/UserService/[controller]")]
[ApiController]
public class UserController(
    IUserService userService,
    IValidator<UserCreateDTO> create,
    IValidator<UserUpdateDTO> update,
    ILogger<UserController> logger
) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IValidator<UserCreateDTO> _create = create;
    private readonly IValidator<UserUpdateDTO> _update = update;
    private readonly ILogger<UserController> _logger = logger;

    [HttpGet]
    [Route("Page")]
    [Produces<ListResult<UserListDTO>>]
    public async Task<IActionResult> GetUsers()
    {
        _logger.LogInformation("User Page (GetUsers) endpoint called");
        var response = await _userService.GetUsersPage();
        return Ok(response);
    }

    [HttpGet]
    [Route("Pk/{userId}")]
    [Produces<UserUpdateDTO>]
    public async Task<IActionResult> GetUserPk(int userId)
    {
        var response = await _userService.GetUserPK(userId);
        return Ok(response);
    }

    [HttpGet]
    [Route("View/{userID}")]
    [Produces<UserViewDTO>]
    public async Task<IActionResult> ViewUser([FromRoute] int userID)
    {
        var user = await _userService.GetUserView(userID);
        var response = ReflectionMapper.Map<UserViewDTO>(user);
        return Ok(response);
    }

    [HttpPost]
    [Route("Insert")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO dto)
    {
        await _create.ValidateAndThrowAsync(dto);
        var response = await _userService.CreateUser(dto);
        return Ok(response);
    }

    [HttpPut]
    [Route("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO dto)
    {
        await _update.ValidateAndThrowAsync(dto);
        var response = await _userService.UpdateUser(dto);
        return Ok(response);
    }

    [HttpDelete]
    [Route("Deactivate/{userid}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeactivateUser([FromRoute] int userid)
    {
        var response = await _userService.DeactivateUser(userid);
        return Ok(response);
    }

    [HttpPost]
    [Route("CreatedAndModifiedBy")]
    [Produces<CreatedAndModifiedDTO>]
    public async Task<IActionResult> CreatedAndModifiedBy(int createdByID, int modifiedByID)
    {
        var response = await _userService.CreatedAndModifiedBy(createdByID, modifiedByID);
        return Ok(response);
    }

    [HttpGet]
    [Route("ResolveAuditUsers")]
    [Produces<AuditUsersDTO>]
    public async Task<IActionResult> ResolveAuditUsers([FromQuery] int createdByID, [FromQuery] int? modifiedByID, [FromQuery] int? approvedByID)
    {
        var response = await _userService.ResolveAuditUsers(createdByID, modifiedByID, approvedByID);
        return Ok(response);
    }
}
