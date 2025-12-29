using Comman.Functions;
using FluentValidation;
using Comman.DTOs.CommanDTOs;
using Comman.MicroserviceDTO;
using Microsoft.AspNetCore.Mvc;
using ProjectGroup.DTOs.UserDTO;
using ProjectGroup.Services.UserService;

namespace ProjectGroup.Controllers;

[Route("api/UserService/[controller]")]
[ApiController]
public class UserController(
        IUserService userService,
        IValidator<UserCreateDTO> Create,
        IValidator<UserUpdateDTO> Update
) : ControllerBase 
{
    #region CONFIGURATION
    private readonly IUserService _userService = userService;
    private readonly IValidator<UserCreateDTO> _create = Create;
    private readonly IValidator<UserUpdateDTO> _update = Update;
    #endregion

    #region GET USERS  PAGE
    [HttpGet]
    [Route("Page")]
    [Produces<ListResult<UserListDTO>>]
    public async Task<IActionResult> GetUsers()
    {
        var response = await _userService.GetUsersPage();
        return Ok(response);
    }
    #endregion

    #region GET USERS BY PK
    [HttpGet]
    [Route("Pk/{UserId}")]
    [Produces<UserUpdateDTO>]
    public async Task<IActionResult> GetUserPk(int UserId)
    {
        var response = await _userService.GetUserPK(UserId);
        return Ok(response);
    }
    #endregion

    #region VIEW USER
    [HttpGet]
    [Route("View/{userID}")]
    [Produces<UserViewDTO>]
    public async Task<IActionResult> ViewUser([FromRoute] int userID)
    {
        var user = await _userService.GetUserView(userID);
        var response = ReflectionMapper.Map<UserViewDTO>(user);
        return Ok(response);
    }
    #endregion

    #region CREATE USER
    [HttpPost]
    [Route("Insert")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO dto)
    {
        await _create.ValidateAndThrowAsync(dto);
        var response = await _userService.CreateUser(dto);
        return Ok(response);
    }
    #endregion

    #region UPDATE USER
    [HttpPut]
    [Route("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateUser(UserUpdateDTO dto)
    {
        await _update.ValidateAndThrowAsync(dto);
        var response = await _userService.UpdateUser(dto);
        return Ok(response);
    }
    #endregion

    #region DEACTIVATE USER
    [HttpDelete]
    [Route("Deactivate/{userid}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeactivateUser([FromRoute] int userid)
    {
        var response = await _userService.DeactivateUser(userid);
        return Ok(response);
    }
    #endregion

    #region CREATED AND MODIFIED BY
    [HttpPost]
    [Route("CreatedAndModifiedBy")]
    [Produces<CreatedAndModifiedDTO>]
    public async Task<IActionResult> CreatedAndModifiedBy(int CreatedByID , int ModifiedByID)
    {
        var response = await _userService.CreatedAndModifiedBy(CreatedByID, ModifiedByID);
        return Ok(response);
    }
    #endregion
}
