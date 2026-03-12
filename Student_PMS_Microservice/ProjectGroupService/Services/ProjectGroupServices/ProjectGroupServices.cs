using Comman.DTOs.CommanDTOs;
using ProjectGroupService.DTOs;
using ProjectGroupService.Repository.ProjectGroup;
using ProjectGroupService.Services.External;

namespace ProjectGroupService.Services.ProjectGroupServices;

public class ProjectGroupService(
    IProjectGroupRepository repository,
    UserServiceClient userServiceClient
) : IProjectGroupServices
{
    public async Task<ListResult<ProjectGroupListDTO>> GetProjectGroupsPage()
    {
        return await repository.GetProjectGroupsPage();
    }

    public async Task<ProjectGroupViewDTO> GetProjectGroupView(int projectGroupID)
    {
        var response = await repository.GetProjectGroupView(projectGroupID);
        var pk = await repository.GetProjectGroupPK(projectGroupID);
        var auditUsers = await userServiceClient.ResolveAuditUsers(pk.CreatedByID, pk.ModifiedByID, pk.ApprovedByID);
        response.CreatedBy = auditUsers.CreatedBy;
        response.ModifiedBy = auditUsers.ModifiedBy;
        response.ApprovedBy = auditUsers.ApprovedBy;
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
