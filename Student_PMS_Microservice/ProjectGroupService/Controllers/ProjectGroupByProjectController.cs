using Comman.DTOs.CommanDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectGroupService.DTOs;
using ProjectGroupService.Services.ProjectGroupByProject;

namespace ProjectGroupService.Controllers;

[Route("api/ProjectGroupService/[controller]")]
[ApiController]
[Authorize]
public class ProjectGroupByProjectController(
    IProjectGroupByProjectService projecjectGroupByProjectService
) : ControllerBase
{
    [HttpGet]
    [Route("Page")]
    [Produces<ListResult<ProjectGroupByProjectListDTO>>]
    public async Task<IActionResult> GetProjectGroupByProjectPage()
    {
        var response = await projecjectGroupByProjectService.GetProjectGroupByProjectsPage();
        return Ok(response);
    }

    [HttpGet]
    [Route("View/{id:int}")]
    [Produces<ProjectGroupByProjectViewDTO>]
    public async Task<IActionResult> GetProjectGroupByProjectView(int id)
    {
        var response = await projecjectGroupByProjectService.GetProjectGroupByProjectView(id);
        return Ok(response);
    }

    [HttpGet]
    [Route("PK/{id:int}")]
    [Produces<ProjectGroupByProjectUpdateDTO>]
    public async Task<IActionResult> GetProjectGroupByProjectPk(int id)
    {
        var response = await projecjectGroupByProjectService.GetProjectGroupByProjectPK(id);
        return Ok(response);
    }

    [HttpPost]
    [Route("Create")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateProjectGroupByProject(ProjectGroupByProjectCreateDTO dto)
    {
        var response = await projecjectGroupByProjectService.CreateProjectGroupByProject(dto);
        return Ok(response);
    }

    [HttpPost]
    [Route("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateProjectGroupByProject([FromBody] ProjectGroupByProjectUpdateDTO dto)
    {
        var response = await projecjectGroupByProjectService.UpdateProjectGroupByProject(dto);
        return Ok(response);
    }

    [HttpDelete]
    [Route("Deactivate/{id:int}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeactivateProjectGroupByProject(int id)
    {
        var response = await projecjectGroupByProjectService.DeactivateProjectGroupByProject(id);
        return Ok(response);
    }
}
