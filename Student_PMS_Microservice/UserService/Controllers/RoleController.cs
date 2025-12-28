using Comman.DTOs.CommanDTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserService.DTOs.RoleDTO;
using UserService.Services.RoleService;

namespace UserService.Controllers;

[Route("api/UserService/[controller]")]
[ApiController]
public class RoleController(
        IRoleService roleService,
        IValidator<RoleCreateDTO> Create,
        IValidator<RoleUpdateDTO> Update
) : ControllerBase
{

    #region CONFIGURATION
        private readonly IRoleService _roleService = roleService;
        private readonly IValidator<RoleCreateDTO> _create = Create;
        private readonly IValidator<RoleUpdateDTO> _update = Update;
    #endregion

    #region GET ROLES
    [HttpGet]
    [Route("Page")]
    [Produces<ListResult<RoleListDTO>>]
    public async Task<IActionResult> GetRoles()
    {
        var response = await _roleService.GetRolesPage();
        return Ok(response);
    }
    #endregion

    #region VIEW ROLE
    [HttpGet]
    [Route("View/{roleID}")]
    [Produces<RoleViewDTO>]
    public async Task<IActionResult> ViewRole([FromRoute] int roleID)
    {
       var response = await _roleService.GetRoleView(roleID);
       return Ok(response);
    }
    #endregion

    #region GET ROLE BY PK
    [HttpGet]
    [Route("Pk/{roleID}")]
    [Produces<RoleUpdateDTO>]
    public async Task<IActionResult> GetRolePK([FromRoute] int roleID)
    {
        var response = await _roleService.GetRolePK(roleID);
        return Ok(response);
    }
    #endregion

    #region GET ROLE DROPDOWN
    [HttpGet]
    [Route("Dropdown")]
    [Produces<OptionDTO>]
    public async Task<IActionResult> GetRoleDropdown()
    {
        var response = await _roleService.GetRoleDropdown();
        return Ok(response);
    }
    #endregion

    #region CREATE ROLE
    [HttpPost]
    [Route("Insert")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateRole([FromBody] RoleCreateDTO dto)
    {
        var response = await _roleService.CreateRole(dto);
        return Ok(response);
    }
    #endregion

    #region UPDATE ROLE
    [HttpPut]
    [Route("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateRole([FromBody] RoleUpdateDTO dto)
    {
        var response = await _roleService.UpdateRole(dto);
        return Ok(response);
    }
    #endregion

    #region DEACTIVATE ROLE
    [HttpDelete]
    [Route("Deactivate/{roleId}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeactivateRole([FromRoute] int roleId)
    {
        var response =  await _roleService.DeactivateRole(roleId);
        return Ok(response);
    }
    #endregion
}
