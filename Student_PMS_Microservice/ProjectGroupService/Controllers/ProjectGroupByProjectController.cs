using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGroupService.DTOs;
using ProjectGroupService.Models;
using ProjectGroupServices.Data;

namespace ProjectGroupService.Controllers;

[Route("api/ProjectGroupService/[controller]")]
[ApiController]
public class ProjectGroupByProjectController : ControllerBase
{
    #region CONFIGURATION
    private readonly AppDbContext _context;
    public ProjectGroupByProjectController(AppDbContext context)
    {
        _context = context;
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT PAGE
    [HttpGet]
    [Route("Page")]
    [Produces<ListResult<ProjectGroupByProjectListDTO>>]
    public async Task<IActionResult> GetProjectGroupByProjectPage()
    {
        var pgbp = await  _context.ProjectGroupByProject
                                                .Select(pgp => new
                                                {
                                                    pgp.ProjectGroupByProjectID,
                                                    ProjectGroupName = pgp.projectGroup.ProjectGroupName,
                                                    pgp.ProjectID,
                                                    pgp.IsActive,
                                                })
                                                .ToListAsync();

        var response = ReflectionMapper.Map<ListResult<ProjectGroupByProjectListDTO>>(pgbp);
        return Ok(response);
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT VIEW
    [HttpGet]
    [Route("View/{id:int}")]
    [Produces<ProjectGroupByProjectViewDTO>]
    public async Task<IActionResult> GetProjectGroupByProjectView(int id)
    {
        var pgbp = await _context.ProjectGroupByProject
                                            .Where(pgp => pgp.ProjectGroupByProjectID == id)
                                            .Select(pgp => new
                                            {
                                                ProjectGroupName = pgp.projectGroup.ProjectGroupName,
                                                pgp.ProjectID,
                                                pgp.Created,
                                                pgp.Modified,
                                                pgp.CreatedByID,
                                                pgp.ModifiedByID,
                                                pgp.IsActive,
                                            })
                                            .FirstOrDefaultAsync();

        if (pgbp == null) return NotFound();

        var response = ReflectionMapper.Map<ProjectGroupByProjectViewDTO>(pgbp);

        return Ok(response);
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT PK
    [HttpGet]
    [Route("PK/{id:int}")]
    [Produces<ProjectGroupByProjectUpdateDTO>]
    public async Task<IActionResult> GetProjectGroupByProjectPk(int id)
    {
        var pgbp = _context.ProjectGroupByProject
                                            .FirstOrDefaultAsync(pgp => pgp.ProjectGroupByProjectID == id);

        if(pgbp == null) return NotFound();

        var response = ReflectionMapper.Map<ProjectGroupByProjectUpdateDTO>(pgbp);

        return Ok(response);
    }
    #endregion

    #region CREATE PROJECT GROUP BY PROJECT
    [HttpPost]
    [Route("Create")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateProjectGroupByProject(ProjectGroupByProjectCreateDTO dto)
    {
        var pgbp = dto.Adapt<ProjectGroupByProject>();
        pgbp.Created = DateTime.UtcNow;

        _context.ProjectGroupByProject.Add(pgbp);

        var rows = await _context.SaveChangesAsync();

        var response = new OperationResultDTO
        {
            Id = pgbp.ProjectGroupByProjectID,
            RowsAffected = rows
        };

        return Ok(response);
    }
    #endregion

    #region UPDATE PROJECT GROUP BY PROJECT
    [HttpPost]
    [Route("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateProjectGroupByProject([FromBody] ProjectGroupByProjectUpdateDTO dto)
    {
        var pgbp = await _context.ProjectGroupByProject
                            .FirstOrDefaultAsync(pgp => pgp.ProjectGroupByProjectID == dto.ProjectGroupByProjectID);

        if(pgbp == null) return NotFound();

        dto.Adapt(pgbp);
        pgbp.Modified = DateTime.UtcNow;

        var rows = await _context.SaveChangesAsync();

        var response = new OperationResultDTO
        {
            Id = pgbp.ProjectGroupByProjectID,
            RowsAffected = rows
        };
        return Ok(response);
    }
    #endregion

    #region DELETE PROJECT GROUP BY PROJECT
    [HttpDelete]
    [Route("Delete/{id:int}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeleteProjectGroupByProject(int id)
    {
        var pgbp = await _context.ProjectGroupByProject.FirstOrDefaultAsync(pgp => pgp.ProjectGroupByProjectID == id);
        if(pgbp == null) return NotFound();
        pgbp.Modified = DateTime.UtcNow;
        pgbp.IsActive = false;
        var rows = await _context.SaveChangesAsync();
        var response = new OperationResultDTO
        {   
            Id = pgbp.ProjectGroupByProjectID,
            RowsAffected = rows
        };
        return Ok(response);
    }
    #endregion
}
