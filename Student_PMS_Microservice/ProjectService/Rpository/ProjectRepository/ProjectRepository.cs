using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using Comman.Functions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectService.Data;
using ProjectService.DTOs;
using ProjectService.Models;

namespace ProjectService.Rpository.ProjectRepository;

public class ProjectRepository(
    AppDbContext context
) : IProjectRepository
{
    private readonly AppDbContext _context = context;

    public async Task<ListResult<ProjectListDTO>> GetProjectsPage()
    {
        var project = await _context.Projects.AsNoTracking().ToListAsync();
        return ReflectionMapper.Map<ListResult<ProjectListDTO>>(project);
    }

    public async Task<ProjectViewDTO> GetProjectView(int projectID)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID)
            ?? throw new NotFoundException("Project not found");

        return ReflectionMapper.Map<ProjectViewDTO>(project);
    }

    public async Task<ProjectUpdateDTO> GetProjectPK(int projectID)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID)
            ?? throw new NotFoundException("Project not found");

        return ReflectionMapper.Map<ProjectUpdateDTO>(project);
    }

    public async Task<OperationResultDTO> CreateProject(ProjectCreateDTO dto)
    {
        var project = dto.Adapt<Project>();
        project.Created = DateTime.UtcNow;
        await _context.Projects.AddAsync(project);
        var rows = await _context.SaveChangesAsync();

        return new OperationResultDTO { Id = project.ProjectID, RowsAffected = rows };
    }

    public async Task<OperationResultDTO> UpdateProject(ProjectUpdateDTO dto)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectID == dto.ProjectID)
            ?? throw new NotFoundException("Project not found");

        dto.Adapt(project);
        project.Modified = DateTime.UtcNow;
        var rows = await _context.SaveChangesAsync();

        return new OperationResultDTO { Id = project.ProjectID, RowsAffected = rows };
    }

    public async Task<OperationResultDTO> DeactivateProject(int projectID)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID)
            ?? throw new NotFoundException("Project not found");

        project.IsActive = false;
        project.Modified = DateTime.UtcNow;
        var rows = await _context.SaveChangesAsync();

        return new OperationResultDTO { Id = project.ProjectID, RowsAffected = rows };
    }

    public async Task<bool> ProjectExists(int projectID)
    {
        return await _context.Projects.AnyAsync(p => p.ProjectID == projectID && p.IsActive);
    }
}
