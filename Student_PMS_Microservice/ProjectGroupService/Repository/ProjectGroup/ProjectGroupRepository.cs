using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ProjectGroupService.DTOs;
using ProjectGroupService.Exceptions;
using ProjectGroupServices.Data;
using System.Text.Json;

namespace ProjectGroupService.Repository.ProjectGroup;

public class ProjectGroupRepository(
    AppDbContext context,
    IDistributedCache cache
) : IProjectGroupRepository
{
    #region GET PROJECT GROUP PAGE
    public async Task<ListResult<ProjectGroupListDTO>> GetProjectGroupsPage()
    {
        #region Cache Implementation
        var cacheKey = $"ProjectGroupPage";

        // 1. Try to fetch from Redis
        var cachedData = await cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            // CACHE HIT: We found it!
            var cachedResponse = JsonSerializer.Deserialize<ListResult<ProjectGroupListDTO>>(cachedData);
            if (cachedResponse is not null)
            {
                return cachedResponse;
            }
        }
        #endregion

        // 2. CACHE MISS: Not in Redis, go to Database
        var projectGroups = await context.ProjectGroup.AsNoTracking().ToListAsync();
        var response = ReflectionMapper.Map<ListResult<ProjectGroupListDTO>>(projectGroups);

        // 3. Store in Redis for next time (expires in 10 minutes)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        string serializedData = JsonSerializer.Serialize(response);
        await cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

        return response;
    }
    #endregion

    #region GET PROJECT GROUP VIEW
    public async Task<ProjectGroupViewDTO> GetProjectGroupView(int projectGroupID)
    {
        var project = context.ProjectGroup.FirstOrDefaultAsync(p => p.ProjectGroupID == projectGroupID)
                                        ?? throw new NotFoundException("Project Group not found");
        var response = ReflectionMapper.Map<ProjectGroupViewDTO>(project);
        return response;
    }
    #endregion

    #region GET PROJECT GROUP PK
    public async Task<ProjectGroupUpdateDTO> GetProjectGroupPK(int projectGroupID)
    {
        var projectGroup = await context.ProjectGroup.FirstOrDefaultAsync(p => p.ProjectGroupID == projectGroupID)
                                            ?? throw new NotFoundException("Project Group not found");
        var response = ReflectionMapper.Map<ProjectGroupUpdateDTO>(projectGroup);
        return response;
    }
    #endregion

    #region CREATE PROJECT GROUP
    public async Task<OperationResultDTO> CreateProjectGroup(ProjectGroupCreateDTO dto)
    {
        var projectGroup = dto.Adapt<Models.ProjectGroup>();
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
                                              ?? throw new NotFoundException("Project Group not found");
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
                                           ?? throw new NotFoundException("Project Group not found");
        projectGroup.IsActive = false;
        projectGroup.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();
        var response = new OperationResultDTO { Id = projectGroup.ProjectGroupID, RowsAffected = rows };
        return response;
    }
    #endregion
}