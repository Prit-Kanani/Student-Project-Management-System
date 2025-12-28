using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectGroupService.DTOs;
using ProjectGroupServices.Data;
using UserService.Exceptions;

namespace ProjectGroupService.Rpository.ProjectGroup;

public class ProjectGroupRepository(
    AppDbContext context
) : IProjectGroupRepository
{
    #region GET PROJECT GROUP PAGE
    public async Task<ListResult<ProjectGroupListDTO>> GetProjectGroupsPage()
    {
        var projectGroups = await context.ProjectGroup.AsNoTracking().ToListAsync();
        var response = ReflectionMapper.Map<ListResult<ProjectGroupListDTO>>(projectGroups);
        return response;
    }
    #endregion

    #region GET PROJECT GROUP VIEW
    public async Task<ProjectGroupViewDTO> GetProjectGroupView(int projectGroupID)
    {
        var project = context.ProjectGroup.FirstOrDefaultAsync(p => p.ProjectGroupID == projectGroupID)
                                        ?? throw new ApiException("Project Group not found", 404);
        var response = ReflectionMapper.Map<ProjectGroupViewDTO>(project);
        return response;
    }
    #endregion

    #region GET PROJECT GROUP PK
    public async Task<ProjectGroupUpdateDTO> GetProjectGroupPK(int projectGroupID)
    {
        var projectGroup = await context.ProjectGroup.FirstOrDefaultAsync(p => p.ProjectGroupID == projectGroupID)
                                            ?? throw new ApiException("Project Group not found", 404);
        var response = ReflectionMapper.Map<ProjectGroupUpdateDTO>(projectGroup);
        return response;
    }
    #endregion

    #region CREATE PROJECT GROUP
    public async Task<OperationResultDTO> CreateProjectGroup(ProjectGroupCreateDTO dto)
    {
        var projectGroup = dto.Adapt<ProjectGroup>();
        projectGroup.Created = DateTime.UtcNow;
        await context.ProjectGroup.AddAsync(projectGroup);
        var rows = await context.SaveChangesAsync();
        var response = new OperationResultDTO{ Id = projectGroup.ProjectGroupID, RowsAffected = rows };
        return response;
    }
    #endregion

    #region UPDATE PROJECT GROUP
    public async Task<OperationResultDTO> UpdateProjectGroup(ProjectGroupUpdateDTO dto)
    {
        var projectGroup = await context.ProjectGroup.FirstOrDefaultAsync(p => p.ProjectGroupID == dto.ProjectGroupID)
                                              ?? throw new ApiException("Project not found", 404);
        dto.Adapt(projectGroup);
        projectGroup.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();
        var response = new OperationResultDTO{ Id = projectGroup.ProjectGroupID, RowsAffected = rows };
        return response;
    }
    #endregion

    #region DEACTIVATE PROJECT GROUP
    public async Task<OperationResultDTO> DeactivateProjectGroup(int projectGroupID)
    {
        var projectGroup = await context.ProjectGroup.FirstOrDefaultAsync(p => p.ProjectGroupID == projectGroupID)
                                           ?? throw new ApiException("Project not found", 404);
        projectGroup.IsActive = false;
        projectGroup.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();
        var response = new OperationResultDTO { Id = projectGroup.ProjectGroupID, RowsAffected = rows };
        return response;
    }
    #endregion
}