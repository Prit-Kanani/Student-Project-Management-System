using Comman.DTOs.CommanDTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectGroup.Services.RoleService;
using UserService.DTOs;

namespace UserService.Controllers;

[Route("api/UserService/[controller]")]
[ApiController]
[Authorize]
public class RoleController(
    IRoleService roleService,
    IValidator<RoleCreateDTO> create,
    IValidator<RoleUpdateDTO> update
) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;
    private readonly IValidator<RoleCreateDTO> _create = create;
    private readonly IValidator<RoleUpdateDTO> _update = update;

    [HttpGet("Page")]
    [Produces<ListResult<RoleListDTO>>]
    public async Task<IActionResult> GetRoles()
    {
        var response = await _roleService.GetRolesPage();
        return Ok(response);
    }

    [HttpGet("View/{roleID}")]
    [Produces<RoleViewDTO>]
    public async Task<IActionResult> ViewRole([FromRoute] int roleID)
    {
        var response = await _roleService.GetRoleView(roleID);
        return Ok(response);
    }

    [HttpGet("Pk/{roleID}")]
    [Produces<RoleUpdateDTO>]
    public async Task<IActionResult> GetRolePK([FromRoute] int roleID)
    {
        var response = await _roleService.GetRolePK(roleID);
        return Ok(response);
    }

    [HttpGet("Dropdown")]
    [Produces<OptionDTO>]
    public async Task<IActionResult> GetRoleDropdown()
    {
        var response = await _roleService.GetRoleDropdown();
        return Ok(response);
    }

    [HttpPost("Insert")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateRole([FromBody] RoleCreateDTO dto)
    {
        await _create.ValidateAndThrowAsync(dto);
        var response = await _roleService.CreateRole(dto);
        return Ok(response);
    }

    [HttpPut("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateRole([FromBody] RoleUpdateDTO dto)
    {
        await _update.ValidateAndThrowAsync(dto);
        var response = await _roleService.UpdateRole(dto);
        return Ok(response);
    }

    [HttpDelete("Deactivate/{roleId}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeactivateRole([FromRoute] int roleId)
    {
        var response = await _roleService.DeactivateRole(roleId);
        return Ok(response);
    }
}
