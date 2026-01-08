using Comman.DTOs.CommanDTOs;
using Microsoft.AspNetCore.Mvc;
using ProjectGroupService.DTOs;
using ProjectGroupService.Services.ProjectGroupByProject;

namespace ProjectGroupService.Controllers;

[Route("api/ProjectGroupService/[controller]")]
[ApiController]
public class ProjectGroupByProjectController(
    IProjectGroupByProjectService projecjectGroupByProjectService
) : ControllerBase
{
    #region GET PROJECT GROUP BY PROJECT PAGE
    [HttpGet]
    [Route("Page")]
    [Produces<ListResult<ProjectGroupByProjectListDTO>>]
    public async Task<IActionResult> GetProjectGroupByProjectPage()
    {
        var response = await projecjectGroupByProjectService.GetProjectGroupByProjectsPage();
        return Ok(response);
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT VIEW
    [HttpGet]
    [Route("View/{id:int}")]
    [Produces<ProjectGroupByProjectViewDTO>]
    public async Task<IActionResult> GetProjectGroupByProjectView(int id)
    {
        var response = await projecjectGroupByProjectService.GetProjectGroupByProjectView(id);

        return Ok(response);
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT PK
    [HttpGet]
    [Route("PK/{id:int}")]
    [Produces<ProjectGroupByProjectUpdateDTO>]
    public async Task<IActionResult> GetProjectGroupByProjectPk(int id)
    {
        var response = await projecjectGroupByProjectService.GetProjectGroupByProjectPK(id);
        return Ok(response);
    }
    #endregion

    #region CREATE PROJECT GROUP BY PROJECT
    [HttpPost]
    [Route("Create")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateProjectGroupByProject(ProjectGroupByProjectCreateDTO dto)
    {
        var response = await projecjectGroupByProjectService.CreateProjectGroupByProject(dto);
        return Ok(response);
    }
    #endregion

    #region UPDATE PROJECT GROUP BY PROJECT
    [HttpPost]
    [Route("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateProjectGroupByProject([FromBody] ProjectGroupByProjectUpdateDTO dto)
    {
        var response = await projecjectGroupByProjectService.UpdateProjectGroupByProject(dto);
        return Ok(response);
    }
    #endregion

    #region DELETE PROJECT GROUP BY PROJECT
    [HttpDelete]
    [Route("Deactivate/{id:int}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeactivateProjectGroupByProject(int id)
    {
        var response = await projecjectGroupByProjectService.DeactivateProjectGroupByProject(id);
        return Ok(response);
    }
    #endregion
}
