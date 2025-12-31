using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Dapper;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectGroupService.Data;
using ProjectGroupService.DTOs;
using ProjectGroupService.Repository.ProjectGroupByProject;
using ProjectGroupServices.Data;
using ProjectService.Exceptions;
using System.Data;

namespace ProjectGroupService.Rpository.ProjectGroupByProject;

public class ProjectGroupByProjectRepository(
    DataContext Dappercontext,
    AppDbContext EFcontext
)   : IProjectGroupByProjectRepository
{
    #region GET PROJECT GROUP BY PROJECT PAGE
    public async Task<ListResult<ProjectGroupByProjectListDTO>> GetProjectGroupByProjectsPage()
    {
        var sql = @"
                Select
                            ProjectGroupByProjectID,
                            ProjectGroupName,
                            ProjectID,
                            IsActive
                From        ProjectGroupByProject
                ORDER BY    ProjectGroupn=Name
                    ";
        var connection = Dappercontext.CreateConnection();
        var projectGroupByProject = await connection.QueryAsync<ProjectGroupByProjectListDTO>(sql);
        var response = ReflectionMapper.Map<ListResult<ProjectGroupByProjectListDTO>>(projectGroupByProject);
        return response;
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT VIEW
    public async Task<ProjectGroupByProjectViewDTO> GetProjectGroupByProjectView(int projectGroupByProjectID)
    {
        var projcectGroupByProject = await EFcontext.ProjectGroupByProject
                                                    .FirstOrDefaultAsync(p => p.ProjectGroupByProjectID == projectGroupByProjectID)
                                                    ?? throw new ApiException("Project Group not found", 404);
        var response = ReflectionMapper.Map<ProjectGroupByProjectViewDTO>(projcectGroupByProject);
        return response;
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT PK
    public async Task<ProjectGroupByProjectUpdateDTO> GetProjectGroupByProjectPK(int projectGroupByProjectID)
    {
        var projectGroupByProject = await EFcontext.ProjectGroupByProject
                                                   .FirstOrDefaultAsync(p => p.ProjectGroupByProjectID == projectGroupByProjectID)
                                                   ?? throw new ApiException("Project Group not found", 404);
        var response = ReflectionMapper.Map<ProjectGroupByProjectUpdateDTO>(projectGroupByProject);
        return response;
    }
    #endregion

    #region CREATE PROJECT GROUP BY PROJECT
    public async Task<OperationResultDTO> CreateProjectGroupByProject(ProjectGroupByProjectCreateDTO dto)
    {
        var projectGroupByProject = ReflectionMapper.Map<Models.ProjectGroupByProject>(dto);
        projectGroupByProject.Created = DateTime.UtcNow;
        await EFcontext.ProjectGroupByProject.AddAsync(projectGroupByProject);
        var rows = await EFcontext.SaveChangesAsync();
        var response = new OperationResultDTO { Id = projectGroupByProject.ProjectGroupID, RowsAffected = rows };
        return response;
    }
    #endregion

    #region UPDATE PROJECT GROUP BY PROJECT
    public async Task<OperationResultDTO> UpdateProjectGroupByProject(ProjectGroupByProjectUpdateDTO dto)
    {
        var projectGroupByProject = await EFcontext.ProjectGroupByProject
                                                   .FirstOrDefaultAsync(p => p.ProjectGroupByProjectID == dto.ProjectGroupByProjectID)
                                                   ?? throw new ApiException("Project Group not found", 404);
        dto.Adapt(projectGroupByProject);
        projectGroupByProject.Modified = DateTime.UtcNow;
        EFcontext.ProjectGroupByProject.Update(projectGroupByProject);
        var rows = await EFcontext.SaveChangesAsync();
        var response = new OperationResultDTO { Id = projectGroupByProject.ProjectGroupByProjectID, RowsAffected = rows };
        return response;
    }
    #endregion

    #region DEACTIVATE PROJECT GROUP BY PROJECT
    public async Task<OperationResultDTO> DeactivateProjectGroupByProject(int projectGroupByProjectID)
    {
        var projectGroupByProject = await EFcontext.ProjectGroupByProject
                                                   .FirstOrDefaultAsync(p => p.ProjectGroupByProjectID == projectGroupByProjectID)
                                                   ?? throw new ApiException("Project Group not found", 404);
        projectGroupByProject.IsActive = false;
        projectGroupByProject.Modified = DateTime.UtcNow;
        EFcontext.ProjectGroupByProject.Update(projectGroupByProject);
        var rows = await EFcontext.SaveChangesAsync();
        var response = new OperationResultDTO { Id = projectGroupByProject.ProjectGroupByProjectID, RowsAffected = rows };
        return response;
    }
    #endregion

    #region BULK CREATE PROJECT GROUP BY PROJECT
    public async Task BulkInsertAsync(List<BulkProjectGroupByProjectCreateDTO> projectGroupByProjects)
    {
        using var connection = Dappercontext.CreateConnection();
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
            table.Rows.Add(pgbp.Created,pgbp.IsActive,pgbp.ProjectGroupID, pgbp.CreatedByID, pgbp.ProjectID);
        }

        await bulkCopy.WriteToServerAsync(table);
    }
    #endregion
}
