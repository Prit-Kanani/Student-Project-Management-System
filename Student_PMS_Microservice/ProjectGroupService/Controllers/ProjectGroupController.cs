using FluentValidation;
using Comman.DTOs.CommanDTOs;
using Microsoft.AspNetCore.Mvc;
using ProjectGroupService.DTOs;
using ProjectGroupService.Validation;
using ProjectGroupService.Services.ProjectGroupServices;

namespace ProjectGroupService.Controllers;

[Route("api/ProjectGroupService/[controller]")]
[ApiController]
public class ProjectGroupController(
    IProjectGroupServices projectGroupService,
    InsertValidation insertValidation,
    UpdateValidation updateValidation
) : ControllerBase
{
    #region GET PROJECT GROUPS PAGE
    [HttpGet]
    [Route("Page")]
    [Produces<ListResult<ProjectGroupListDTO>>]
    public async Task<IActionResult> GetProjectGroups()
    {
        var response = await projectGroupService.GetProjectGroupsPage();
        return Ok(response);
    }
    #endregion

    #region GET PROJECT GROUP BY PK
    [HttpGet]
    [Route("PK/{id:int}")]
    [Produces<ProjectGroupUpdateDTO>]
    public async Task<IActionResult> GetProjectGroupPK(int id)
    {
        var response = await projectGroupService.GetProjectGroupPK(id);
        return Ok(response);
    }
    #endregion

    #region VIEW PROJECT GROUP
    [HttpGet]
    [Route("View/{id:int}")]
    public async Task<IActionResult> GetProjectGroupView([FromRoute] int id)
    {
        var response = await projectGroupService.GetProjectGroupView(id);
        return Ok(response);
    }
    #endregion

    #region CREATE PROJECT GROUP
    [HttpPost]
    [Route("Create")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateProjectGroup([FromBody] ProjectGroupCreateDTO dto)
    {
        await insertValidation.ValidateAndThrowAsync(dto);
        var response = await projectGroupService.CreateProjectGroup(dto);
        return Ok(response);
    }
    #endregion

    #region UPDATE PROJECT GROUP
    [HttpPost]
    [Route("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateProjectGroup([FromBody] ProjectGroupUpdateDTO dto)
    {
        await updateValidation.ValidateAndThrowAsync(dto);
        var response = await projectGroupService.UpdateProjectGroup(dto);
        return Ok(response);
    }
    #endregion

    #region DELETE PROJECT GROUP
    [HttpDelete]
    [Route("Delete/{id:int}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeactivateProjectGroup(int id)
    {
        var response = await projectGroupService.DeactivateProjectGroup(id);
        return Ok(response);
    }
    #endregion
}
