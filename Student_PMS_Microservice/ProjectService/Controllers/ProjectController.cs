using FluentValidation;
using ProjectService.DTOs;
using Comman.DTOs.CommanDTOs;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Validation;
using ProjectService.Services.ProjectServices;

namespace ProjectService.Controllers;

[Route("api/ProjectService/[controller]")]
[ApiController]
public class ProjectController(
    IProjectServices projectServices,
    InsertValidation insertvalidator,
    UpdateValidation updatevalidator
) : ControllerBase
{
    #region GET PROJECT PAGE
    [HttpGet]
    [Route("Page")]
    [Produces<ListResult<ProjectListDTO>>]
    public async Task<IActionResult> GetProjectPage()
    {
        var response = await projectServices.GetProjectsPage();
        return Ok(response);
    }
    #endregion

    #region GET PROJECT VIEW
    [HttpGet]
    [Route("View/{id:int}")]
    [Produces<ProjectViewDTO>]
    public async Task<IActionResult> GetProjectView(int id)
    {
        var response = await projectServices.GetProjectView(id);
        return Ok(response);
    }
    #endregion

    #region GET PROJECT PK
    [HttpGet]
    [Route("PK/{id:int}")]
    [Produces<ProjectViewDTO>]
    public async Task<IActionResult> GetProjectPk(int id)
    {
        var response = await projectServices.GetProjectPK(id);
        return Ok(response);
    }
    #endregion

    #region CREATE PROJECT 
    [HttpPost]
    [Route("Create")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateProject(ProjectCreateDTO dto)
    {
        await insertvalidator.ValidateAndThrowAsync(dto);
        var response = await projectServices.CreateProject(dto);
        return Ok(response);
    }
    #endregion

    #region UPDATE PROJECT
    [HttpPost]
    [Route("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateProject(ProjectUpdateDTO dto)
    {
        await updatevalidator.ValidateAndThrowAsync(dto);
        var response = await projectServices.UpdateProject(dto);
        return Ok(response);
    }
    #endregion

    #region DELETE PROJECT
    [HttpDelete]
    [Route("Delete/{id:int}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeactivateProject(int id)
    {
        var response = await projectServices.DeactivateProject(id);
        return Ok(response);
    }
    #endregion
}