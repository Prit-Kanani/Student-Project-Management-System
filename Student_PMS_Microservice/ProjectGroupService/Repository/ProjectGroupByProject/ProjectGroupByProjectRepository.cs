using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Dapper;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectGroupService.Data;
using ProjectGroupService.DTOs;
using ProjectGroupService.Exceptions;
using ProjectGroupServices.Data;
using System.Data;

namespace ProjectGroupService.Repository.ProjectGroupByProject;

public class ProjectGroupByProjectRepository(
    DataContext Dappercontext,
    AppDbContext EFcontext
)   : IProjectGroupByProjectRepository
{
    #region GET PROJECT GROUP BY PROJECT PAGE
    public async Task<ListResult<ProjectGroupByProjectListDTO>> GetProjectGroupByProjectsPage()
    {
        var pr = @"PR_ProjectGroupByProjectService_SelectPage";
        var connection = Dappercontext.CreateConnection();
        var projectGroupByProject = await connection.QueryAsync<ProjectGroupByProjectListDTO>
                                                    (pr, commandType: CommandType.StoredProcedure);
        var response = ReflectionMapper.Map<ListResult<ProjectGroupByProjectListDTO>>(projectGroupByProject);
        return response;
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT VIEW
    public async Task<ProjectGroupByProjectViewDTO> GetProjectGroupByProjectView(int projectGroupByProjectID)
    {
        var pr = @"PR_ProjectGroupByProjectService_SelectView";
        var connection = Dappercontext.CreateConnection();
        var projectGroupByProject = await connection.QueryAsync<ProjectGroupByProjectViewDTO>
                                                    (pr, new { projectGroupByProjectID }, commandType: CommandType.StoredProcedure)
                                                    ?? throw new NotFoundException("Project Group By Project not found");
        var response = ReflectionMapper.Map<ProjectGroupByProjectViewDTO>(projectGroupByProject);
        return response;
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT PK
    public async Task<ProjectGroupByProjectUpdateDTO> GetProjectGroupByProjectPK(int projectGroupByProjectID)
    {
        var pr = @"PR_ProjectGroupByProjectService_SelectPK";
        var connection = Dappercontext.CreateConnection();
        var projectGroupByProject = await connection.QueryAsync<ProjectGroupByProjectUpdateDTO>
                                                    (pr, new { projectGroupByProjectID }, commandType: CommandType.StoredProcedure)
                                                   ?? throw new NotFoundException("Project Group not found");
        var response = ReflectionMapper.Map<ProjectGroupByProjectUpdateDTO>(projectGroupByProject);
        return response;
    }
    #endregion

    #region CREATE PROJECT GROUP BY PROJECT
    public async Task<OperationResultDTO> CreateProjectGroupByProject(ProjectGroupByProjectCreateDTO dto)
    {
        var parameters = new DynamicParameters(dto);
        parameters.Add("@ProjectGroupByProjectID", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@RowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);
        var connection = Dappercontext.CreateConnection();
        await connection.ExecuteAsync("PR_ProjectGroupByProjectService_Create", parameters, commandType: CommandType.StoredProcedure );

        var id = parameters.Get<int>("@ProjectGroupByProjectID");
        var rows = parameters.Get<int>("@RowsAffected");

        if (id == 0 || rows == 0)  throw new NotFoundException("Project Group not found");
        var response = new OperationResultDTO { Id = id, RowsAffected = rows };
        return response;
    }
    #endregion

    #region UPDATE PROJECT GROUP BY PROJECT
    public async Task<OperationResultDTO> UpdateProjectGroupByProject(ProjectGroupByProjectUpdateDTO dto)
    {
        var projectGroupByProject = await EFcontext.ProjectGroupByProject
                                                   .FirstOrDefaultAsync(p => p.ProjectGroupByProjectID == dto.ProjectGroupByProjectID)
                                                   ?? throw new NotFoundException("Project Group not found");
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
                                                   ?? throw new NotFoundException("Project Group not found");
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

        var table = new DataTable();
        table.Columns.Add("ProjectGroupID", typeof(int));
        table.Columns.Add("CreatedByID", typeof(int));
        table.Columns.Add("ProjectID", typeof(int));
        table.Columns.Add("IsActive", typeof(bool));

        foreach (var pgbp in projectGroupByProjects)
        {
            table.Rows.Add(pgbp.IsActive,pgbp.ProjectGroupID, pgbp.CreatedByID, pgbp.ProjectID);
        }

        await bulkCopy.WriteToServerAsync(table);
    }
    #endregion
}
