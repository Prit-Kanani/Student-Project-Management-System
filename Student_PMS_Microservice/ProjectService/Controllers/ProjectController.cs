using Comman.DTOs.CommanDTOs;
using Comman.MicroserviceDTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProjectService.DTOs;
using ProjectService.Services.ProjectServices;
using ProjectService.Validation;

namespace ProjectService.Controllers;

[Route("api/ProjectService/[controller]")]
[ApiController]
public class ProjectController(
    IProjectServices projectServices,
    InsertValidation insertvalidator,
    UpdateValidation updatevalidator
) : ControllerBase
{
    [HttpGet]
    [Route("Page")]
    [Produces<ListResult<ProjectListDTO>>]
    public async Task<IActionResult> GetProjectPage()
    {
        var response = await projectServices.GetProjectsPage();
        return Ok(response);
    }

    [HttpGet]
    [Route("View/{id:int}")]
    [Produces<ProjectViewDTO>]
    public async Task<IActionResult> GetProjectView(int id)
    {
        var response = await projectServices.GetProjectView(id);
        return Ok(response);
    }

    [HttpGet]
    [Route("PK/{id:int}")]
    [Produces<ProjectViewDTO>]
    public async Task<IActionResult> GetProjectPk(int id)
    {
        var response = await projectServices.GetProjectPK(id);
        return Ok(response);
    }

    [HttpGet]
    [Route("Exists/{id:int}")]
    [Produces<EntityExistsDTO>]
    public async Task<IActionResult> ProjectExists(int id)
    {
        return Ok(await projectServices.ProjectExists(id));
    }

    [HttpPost]
    [Route("Create")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateProject([FromBody] ProjectCreateDTO dto)
    {
        await insertvalidator.ValidateAndThrowAsync(dto);
        var response = await projectServices.CreateProject(dto);
        return Ok(response);
    }

    [HttpPut]
    [Route("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateProject([FromBody] ProjectUpdateDTO dto)
    {
        await updatevalidator.ValidateAndThrowAsync(dto);
        var response = await projectServices.UpdateProject(dto);
        return Ok(response);
    }

    [HttpDelete]
    [Route("Delete/{id:int}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeactivateProject(int id)
    {
        var response = await projectServices.DeactivateProject(id);
        return Ok(response);
    }
}
