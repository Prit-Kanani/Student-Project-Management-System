using Comman.DTOs.CommanDTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectGroupService.DTOs;
using ProjectGroupService.Services.ProjectGroupServices;
using ProjectGroupService.Validation;

namespace ProjectGroupService.Controllers;

[Route("api/ProjectGroupService/[controller]")]
[ApiController]
[Authorize]
public class ProjectGroupController(
    IProjectGroupServices projectGroupService,
    InsertValidation insertValidation,
    UpdateValidation updateValidation
) : ControllerBase
{
    [HttpGet]
    [Route("Page")]
    [Produces<ListResult<ProjectGroupListDTO>>]
    public async Task<IActionResult> GetProjectGroups()
    {
        var response = await projectGroupService.GetProjectGroupsPage();
        return Ok(response);
    }

    [HttpGet]
    [Route("PK/{id:int}")]
    [Produces<ProjectGroupUpdateDTO>]
    public async Task<IActionResult> GetProjectGroupPK(int id)
    {
        var response = await projectGroupService.GetProjectGroupPK(id);
        return Ok(response);
    }

    [HttpGet]
    [Route("View/{id:int}")]
    public async Task<IActionResult> GetProjectGroupView([FromRoute] int id)
    {
        var response = await projectGroupService.GetProjectGroupView(id);
        return Ok(response);
    }

    [HttpPost]
    [Route("Create")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateProjectGroup([FromBody] ProjectGroupCreateDTO dto)
    {
        await insertValidation.ValidateAndThrowAsync(dto);
        var response = await projectGroupService.CreateProjectGroup(dto);
        return Ok(response);
    }

    [HttpPost]
    [Route("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateProjectGroup([FromBody] ProjectGroupUpdateDTO dto)
    {
        await updateValidation.ValidateAndThrowAsync(dto);
        var response = await projectGroupService.UpdateProjectGroup(dto);
        return Ok(response);
    }

    [HttpDelete]
    [Route("Deactivate/{id:int}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeactivateProjectGroup(int id)
    {
        var response = await projectGroupService.DeactivateProjectGroup(id);
        return Ok(response);
    }
}
