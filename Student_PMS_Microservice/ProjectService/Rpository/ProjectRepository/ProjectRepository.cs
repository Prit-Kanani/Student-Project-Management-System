using Mapster;
using Comman.Functions;
using ProjectService.Data;
using ProjectService.DTOs;
using ProjectService.Models;
using Comman.DTOs.CommanDTOs;
using ProjectService.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ProjectService.Rpository.ProjectRepository;

public class ProjectRepository(
    AppDbContext context
) : IProjectRepository
{
    #region CONFIGURATION
    private readonly AppDbContext _context = context;
    #endregion

    #region GET PROJECT PAGE
    public async Task<ListResult<ProjectListDTO>> GetProjectsPage()
    {
        var project = await _context.Projects.AsNoTracking().ToListAsync();
        var response = ReflectionMapper.Map<ListResult<ProjectListDTO>>(project);
        return response;
    }
    #endregion

    #region GET PROJECT VIEW
    public async Task<ProjectViewDTO> GetProjectView(int projectID)
    {
        var project = _context.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID)
                                        ?? throw new NotFoundException("Project not found");
        var response = ReflectionMapper.Map<ProjectViewDTO>(project);
        return response;
    }
    #endregion

    #region GET PROJECT PK
    public async Task<ProjectUpdateDTO> GetProjectPK(int projectID)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID)
                                            ?? throw new NotFoundException("Project not found");
        var response = ReflectionMapper.Map<ProjectUpdateDTO>(project);
        return response;
    }
    #endregion

    #region CREATE PROJECT
    public async Task<OperationResultDTO> CreateProject(ProjectCreateDTO dto)
    {
        var project = dto.Adapt<Project>();
        project.Created = DateTime.UtcNow;
        await _context.Projects.AddAsync(project);
        var rows = await _context.SaveChangesAsync();
        var response = new OperationResultDTO{ Id = project.ProjectID , RowsAffected = rows };
        return response;
    }
    #endregion

    #region UPDATE PROJECT
    public async Task<OperationResultDTO> UpdateProject(ProjectUpdateDTO dto)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectID == dto.ProjectID)
                                              ?? throw new NotFoundException("Project not found");
        dto.Adapt(project);
        project.Modified = DateTime.UtcNow;
        var rows = await _context.SaveChangesAsync();
        var response = new OperationResultDTO{ Id = project.ProjectID, RowsAffected = rows };
        return response;
    }
    #endregion

    #region DEACTIVATE USER
    public async Task<OperationResultDTO> DeactivateProject(int projectID)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID)
                                           ?? throw new NotFoundException("Project not found");
        project.IsActive = false;
        project.Modified = DateTime.UtcNow;
        var rows = await _context.SaveChangesAsync();
        var response = new OperationResultDTO { Id = project.ProjectID, RowsAffected = rows };
        return response;
    }
    #endregion
}