using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using ProjectGroupService.DTOs;
using ProjectGroupService.Repository.ProjectGroupByProject;
using ProjectGroupService.Services.External;

namespace ProjectGroupService.Services.ProjectGroupByProject;

public class ProjectGroupByProjectService(
    IProjectGroupByProjectRepository repository,
    UserServiceClient userServiceClient,
    ProjectServiceClient projectServiceClient,
    ILogger<ProjectGroupByProjectService> logger
) : IProjectGroupByProjectService
{
    public async Task<ListResult<ProjectGroupByProjectListDTO>> GetProjectGroupByProjectsPage()
    {
        return await repository.GetProjectGroupByProjectsPage();
    }

    public async Task<ProjectGroupByProjectViewDTO> GetProjectGroupByProjectView(int projectGroupByProjectID)
    {
        var response = await repository.GetProjectGroupByProjectView(projectGroupByProjectID);

        try
        {
            var auditUsers = await userServiceClient.ResolveAuditUsers(
                response.CreatedByID,
                response.ModifiedByID,
                null
            );

            response.CreatedBy = auditUsers.CreatedBy;
            response.ModifiedBy = auditUsers.ModifiedBy;
        }
        catch (Exception ex) when (
            ex is ApiException ||
            ex is HttpRequestException ||
            ex is TaskCanceledException
        )
        {
            logger.LogWarning(
                ex,
                "Audit user resolution failed for project-group mapping {ProjectGroupByProjectID}. Returning the core payload without audit names.",
                projectGroupByProjectID
            );
        }

        return response;
    }

    public async Task<ProjectGroupByProjectUpdateDTO> GetProjectGroupByProjectPK(int projectGroupByProjectID)
    {
        return await repository.GetProjectGroupByProjectPK(projectGroupByProjectID);
    }

    public async Task<OperationResultDTO> CreateProjectGroupByProject(ProjectGroupByProjectCreateDTO dto)
    {
        await projectServiceClient.EnsureProjectExists(dto.ProjectID);
        return await repository.CreateProjectGroupByProject(dto);
    }

    public async Task<OperationResultDTO> UpdateProjectGroupByProject(ProjectGroupByProjectUpdateDTO dto)
    {
        await projectServiceClient.EnsureProjectExists(dto.ProjectID);
        return await repository.UpdateProjectGroupByProject(dto);
    }

    public async Task<OperationResultDTO> DeactivateProjectGroupByProject(int projectGroupByProjectID)
    {
        return await repository.DeactivateProjectGroupByProject(projectGroupByProjectID);
    }

    public async Task BulkInsertAsync(List<BulkProjectGroupByProjectCreateDTO> projectGroupByProjects)
    {
        foreach (var item in projectGroupByProjects)
        {
            await projectServiceClient.EnsureProjectExists(item.ProjectID);
        }

        await repository.BulkInsertAsync(projectGroupByProjects);
    }
}
