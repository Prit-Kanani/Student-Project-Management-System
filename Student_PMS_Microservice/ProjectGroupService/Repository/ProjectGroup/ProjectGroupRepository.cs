using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using Comman.Functions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ProjectGroupService.DTOs;
using ProjectGroupServices.Data;
using System.Text.Json;

namespace ProjectGroupService.Repository.ProjectGroup;

public class ProjectGroupRepository(
    AppDbContext context,
    IDistributedCache cache,
    ILogger<ProjectGroupRepository> logger
) : IProjectGroupRepository
{
    private const string ProjectGroupPageCacheKey = "ProjectGroupPage";

    public async Task<ListResult<ProjectGroupListDTO>> GetProjectGroupsPage()
    {
        var cachedResponse = await TryGetCachedPage();
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var projectGroups = await context.ProjectGroup.AsNoTracking().ToListAsync();
        var response = ReflectionMapper.Map<ListResult<ProjectGroupListDTO>>(projectGroups);

        await TrySetCachedPage(response);
        return response;
    }

    public async Task<ProjectGroupViewDTO> GetProjectGroupView(int projectGroupID)
    {
        var project = await context.ProjectGroup.FirstOrDefaultAsync(p => p.ProjectGroupID == projectGroupID)
            ?? throw new NotFoundException("Project Group not found");

        return ReflectionMapper.Map<ProjectGroupViewDTO>(project);
    }

    public async Task<ProjectGroupUpdateDTO> GetProjectGroupPK(int projectGroupID)
    {
        var projectGroup = await context.ProjectGroup.FirstOrDefaultAsync(p => p.ProjectGroupID == projectGroupID)
            ?? throw new NotFoundException("Project Group not found");

        return ReflectionMapper.Map<ProjectGroupUpdateDTO>(projectGroup);
    }

    public async Task<OperationResultDTO> CreateProjectGroup(ProjectGroupCreateDTO dto)
    {
        var projectGroup = dto.Adapt<Models.ProjectGroup>();
        projectGroup.Created = DateTime.UtcNow;
        await context.ProjectGroup.AddAsync(projectGroup);
        var rows = await context.SaveChangesAsync();
        await TryRemovePageCache();

        return new OperationResultDTO
        {
            Id = projectGroup.ProjectGroupID,
            RowsAffected = rows
        };
    }

    public async Task<OperationResultDTO> UpdateProjectGroup(ProjectGroupUpdateDTO dto)
    {
        var projectGroup = await context.ProjectGroup.FirstOrDefaultAsync(p => p.ProjectGroupID == dto.ProjectGroupID)
            ?? throw new NotFoundException("Project Group not found");

        dto.Adapt(projectGroup);
        projectGroup.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();
        await TryRemovePageCache();

        return new OperationResultDTO
        {
            Id = projectGroup.ProjectGroupID,
            RowsAffected = rows
        };
    }

    public async Task<OperationResultDTO> DeactivateProjectGroup(int projectGroupID)
    {
        var projectGroup = await context.ProjectGroup.FirstOrDefaultAsync(p => p.ProjectGroupID == projectGroupID)
            ?? throw new NotFoundException("Project Group not found");

        projectGroup.IsActive = false;
        projectGroup.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();
        await TryRemovePageCache();

        return new OperationResultDTO
        {
            Id = projectGroup.ProjectGroupID,
            RowsAffected = rows
        };
    }

    private async Task<ListResult<ProjectGroupListDTO>?> TryGetCachedPage()
    {
        try
        {
            var cachedData = await cache.GetStringAsync(ProjectGroupPageCacheKey);
            if (string.IsNullOrEmpty(cachedData))
            {
                return null;
            }

            return JsonSerializer.Deserialize<ListResult<ProjectGroupListDTO>>(cachedData);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "ProjectGroup page cache read failed. Falling back to the database.");
            return null;
        }
    }

    private async Task TrySetCachedPage(ListResult<ProjectGroupListDTO> response)
    {
        try
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            var serializedData = JsonSerializer.Serialize(response);
            await cache.SetStringAsync(ProjectGroupPageCacheKey, serializedData, cacheOptions);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "ProjectGroup page cache write failed. Returning uncached response.");
        }
    }

    private async Task TryRemovePageCache()
    {
        try
        {
            await cache.RemoveAsync(ProjectGroupPageCacheKey);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "ProjectGroup page cache invalidation failed after a write operation.");
        }
    }
}
