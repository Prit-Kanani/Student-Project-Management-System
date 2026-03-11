using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Dapper;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using ProjectGroupService.Data;
using ProjectGroupService.DTOs;
using ProjectGroupService.Exceptions;
using System.Data;
using System.Security.Claims;

namespace ProjectGroupService.Repository.ProjectGroupByProject;

public class ProjectGroupByProjectRepository(
    DataContext dapperContext,
    IMemoryCache cache,
    IHttpContextAccessor httpContextAccessor
) : IProjectGroupByProjectRepository
{
    private readonly IMemoryCache _cache = cache;

    private static readonly MemoryCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
        SlidingExpiration = TimeSpan.FromMinutes(3),
        Priority = CacheItemPriority.Normal
    };

    public async Task<ListResult<ProjectGroupByProjectListDTO>> GetProjectGroupByProjectsPage()
    {
        if (_cache.TryGetValue(ProjectGroupByProjectCacheKeys.Page, out ListResult<ProjectGroupByProjectListDTO> cached))
        {
            return cached;
        }

        const string pr = "PR_ProjectGroupByProjectService_SelectPage";
        using var connection = dapperContext.CreateConnection();

        var data = await connection.QueryAsync<ProjectGroupByProjectListDTO>(
            pr,
            commandType: CommandType.StoredProcedure);

        var response = ReflectionMapper.Map<ListResult<ProjectGroupByProjectListDTO>>(data);
        _cache.Set(ProjectGroupByProjectCacheKeys.Page, response, CacheOptions);

        return response;
    }

    public async Task<ProjectGroupByProjectViewDTO> GetProjectGroupByProjectView(int projectGroupByProjectID)
    {
        var key = ProjectGroupByProjectCacheKeys.View(projectGroupByProjectID);

        if (_cache.TryGetValue(key, out ProjectGroupByProjectViewDTO cached))
        {
            return cached;
        }

        const string pr = "PR_ProjectGroupByProjectService_SelectView";
        using var connection = dapperContext.CreateConnection();

        var data = await connection.QueryFirstOrDefaultAsync<ProjectGroupByProjectViewDTO>(
            pr,
            new { projectGroupByProjectID },
            commandType: CommandType.StoredProcedure)
            ?? throw new NotFoundException("Project Group By Project not found");

        _cache.Set(key, data, CacheOptions);
        return data;
    }

    public async Task<ProjectGroupByProjectUpdateDTO> GetProjectGroupByProjectPK(int projectGroupByProjectID)
    {
        var key = ProjectGroupByProjectCacheKeys.PK(projectGroupByProjectID);

        if (_cache.TryGetValue(key, out ProjectGroupByProjectUpdateDTO cached))
        {
            return cached;
        }

        const string pr = "PR_ProjectGroupByProjectService_SelectPK";
        using var connection = dapperContext.CreateConnection();

        var data = await connection.QueryFirstOrDefaultAsync<ProjectGroupByProjectUpdateDTO>(
            pr,
            new { projectGroupByProjectID },
            commandType: CommandType.StoredProcedure)
            ?? throw new NotFoundException("Project Group not found");

        _cache.Set(key, data, CacheOptions);
        return data;
    }

    public async Task<OperationResultDTO> CreateProjectGroupByProject(ProjectGroupByProjectCreateDTO dto)
    {
        var parameters = new DynamicParameters(dto);
        parameters.Add("@ProjectGroupByProjectID", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@RowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var connection = dapperContext.CreateConnection();

        try
        {
            await connection.ExecuteAsync(
                "PR_ProjectGroupByProjectService_Create",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex)
        {
            throw ex.Number switch
            {
                2627 or 2601 => new DuplicateKeyException("Project Group By Project already exists", ex.Number),
                547 => new ForeignKeyViolationException("Invalid ProjectGroupID or ProjectID reference", ex.Number),
                -2 => new DatabaseTimeoutException("Database operation timed out", ex.Number),
                _ => new DatabaseServerException("Database error occurred while creating Project Group By Project")
            };
        }

        var id = parameters.Get<int>("@ProjectGroupByProjectID");
        var rows = parameters.Get<int>("@RowsAffected");

        if (id <= 0)
        {
            throw new DatabaseServerException("Database error occurred while creating Project Group By Project");
        }

        _cache.Remove(ProjectGroupByProjectCacheKeys.Page);
        return new OperationResultDTO { Id = id, RowsAffected = rows };
    }

    public async Task<OperationResultDTO> UpdateProjectGroupByProject(ProjectGroupByProjectUpdateDTO dto)
    {
        var parameters = new DynamicParameters(dto);
        parameters.Add("@RowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var connection = dapperContext.CreateConnection();

        try
        {
            await connection.ExecuteAsync(
                "PR_ProjectGroupByProjectService_Update",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex)
        {
            throw ex.Number switch
            {
                2627 or 2601 => new DuplicateKeyException("Duplicate Project Group By Project record", ex.Number),
                547 => new ForeignKeyViolationException("Invalid ProjectGroupID or ProjectID reference", ex.Number),
                -2 => new DatabaseTimeoutException("Database operation timed out", ex.Number),
                _ => new DatabaseServerException("Database error occurred while updating Project Group By Project")
            };
        }

        var rows = parameters.Get<int>("@RowsAffected");
        if (rows == 0)
        {
            throw new NotFoundException("Project Group By Project not found or no changes detected");
        }

        _cache.Remove(ProjectGroupByProjectCacheKeys.Page);
        _cache.Remove(ProjectGroupByProjectCacheKeys.View(dto.ProjectGroupByProjectID));
        _cache.Remove(ProjectGroupByProjectCacheKeys.PK(dto.ProjectGroupByProjectID));

        return new OperationResultDTO
        {
            Id = dto.ProjectGroupByProjectID,
            RowsAffected = rows
        };
    }

    public async Task<OperationResultDTO> DeactivateProjectGroupByProject(int projectGroupByProjectID)
    {
        var modifiedByID = GetModifiedById(httpContextAccessor);

        var parameters = new DynamicParameters();
        parameters.Add("@ProjectGroupByProjectID", projectGroupByProjectID);
        parameters.Add("@ModifiedByID", modifiedByID);
        parameters.Add("@RowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var connection = dapperContext.CreateConnection();

        try
        {
            await connection.ExecuteAsync(
                "PR_ProjectGroupByProjectService_Deactivate",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex)
        {
            throw ex.Number switch
            {
                547 => new ForeignKeyViolationException("Cannot deactivate Project Group By Project due to references", ex.Number),
                -2 => new DatabaseTimeoutException("Database operation timed out", ex.Number),
                _ => new DatabaseServerException("Database error occurred while deactivating Project Group By Project")
            };
        }

        var rows = parameters.Get<int>("@RowsAffected");
        if (rows == 0)
        {
            throw new NotFoundException("Project Group By Project not found or already deactivated");
        }

        _cache.Remove(ProjectGroupByProjectCacheKeys.Page);
        _cache.Remove(ProjectGroupByProjectCacheKeys.View(projectGroupByProjectID));
        _cache.Remove(ProjectGroupByProjectCacheKeys.PK(projectGroupByProjectID));

        return new OperationResultDTO
        {
            Id = projectGroupByProjectID,
            RowsAffected = rows
        };
    }

    public async Task BulkInsertAsync(List<BulkProjectGroupByProjectCreateDTO> projectGroupByProjects)
    {
        using var connection = dapperContext.CreateConnection();
        await connection.OpenAsync();

        using var bulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = "dbo.ProjectGroupByProject",
            BatchSize = 1000,
            BulkCopyTimeout = 60
        };

        bulkCopy.ColumnMappings.Add("ProjectGroupID", "ProjectGroupID");
        bulkCopy.ColumnMappings.Add("CreatedByID", "CreatedByID");
        bulkCopy.ColumnMappings.Add("ProjectID", "ProjectID");
        bulkCopy.ColumnMappings.Add("IsActive", "IsActive");
        bulkCopy.ColumnMappings.Add("Created", "Created");

        var table = new DataTable();
        table.Columns.Add("ProjectGroupID", typeof(int));
        table.Columns.Add("CreatedByID", typeof(int));
        table.Columns.Add("ProjectID", typeof(int));
        table.Columns.Add("IsActive", typeof(bool));
        table.Columns.Add("Created", typeof(DateTime));

        foreach (var pgbp in projectGroupByProjects)
        {
            table.Rows.Add(pgbp.ProjectGroupID, pgbp.CreatedByID, pgbp.ProjectID, pgbp.IsActive, DateTime.UtcNow);
        }

        await bulkCopy.WriteToServerAsync(table);
        _cache.Remove(ProjectGroupByProjectCacheKeys.Page);
        await connection.CloseAsync();
    }

    private static int GetModifiedById(IHttpContextAccessor accessor)
    {
        var user = accessor.HttpContext?.User;
        var claimValue = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? user?.FindFirst("sub")?.Value;

        if (int.TryParse(claimValue, out var userId))
        {
            return userId;
        }

        throw new UnauthorizedException("Authenticated user id is required");
    }
}

public static class ProjectGroupByProjectCacheKeys
{
    public const string Page = "PGBP:PAGE";
    public static string View(int id) => $"PGBP:VIEW:{id}";
    public static string PK(int id) => $"PGBP:PK:{id}";
}
