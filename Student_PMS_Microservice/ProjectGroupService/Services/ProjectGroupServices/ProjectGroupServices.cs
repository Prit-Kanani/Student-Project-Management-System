using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using ProjectGroupService.DTOs;
using ProjectGroupService.Repository.ProjectGroup;
using ProjectGroupService.Services.External;

namespace ProjectGroupService.Services.ProjectGroupServices;

public class ProjectGroupService(
    IProjectGroupRepository repository,
    UserServiceClient userServiceClient,
    ILogger<ProjectGroupService> logger
) : IProjectGroupServices
{
    public async Task<ListResult<ProjectGroupListDTO>> GetProjectGroupsPage()
    {
        return await repository.GetProjectGroupsPage();
    }

    public async Task<ProjectGroupViewDTO> GetProjectGroupView(int projectGroupID)
    {
        var response = await repository.GetProjectGroupView(projectGroupID);

        try
        {
            var pk = await repository.GetProjectGroupPK(projectGroupID);
            var auditUsers = await userServiceClient.ResolveAuditUsers(
                pk.CreatedByID,
                pk.ModifiedByID,
                pk.ApprovedByID
            );

            response.CreatedBy = auditUsers.CreatedBy;
            response.ModifiedBy = auditUsers.ModifiedBy;
            response.ApprovedBy = auditUsers.ApprovedBy;
        }
        catch (Exception ex) when (
            ex is ApiException ||
            ex is HttpRequestException ||
            ex is TaskCanceledException
        )
        {
            logger.LogWarning(
                ex,
                "Audit user resolution failed for project group {ProjectGroupID}. Returning the core project group payload without audit names.",
                projectGroupID
            );
        }

        return response;
    }

    public async Task<ProjectGroupUpdateDTO> GetProjectGroupPK(int projectGroupID)
    {
        return await repository.GetProjectGroupPK(projectGroupID);
    }

    public async Task<OperationResultDTO> CreateProjectGroup(ProjectGroupCreateDTO dto)
    {
        return await repository.CreateProjectGroup(dto);
    }

    public async Task<OperationResultDTO> UpdateProjectGroup(ProjectGroupUpdateDTO dto)
    {
        return await repository.UpdateProjectGroup(dto);
    }

    public async Task<OperationResultDTO> DeactivateProjectGroup(int projectGroupID)
    {
        return await repository.DeactivateProjectGroup(projectGroupID);
    }
}
