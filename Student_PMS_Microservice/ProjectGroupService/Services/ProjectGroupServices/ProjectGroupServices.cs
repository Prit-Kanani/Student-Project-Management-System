using Comman.DTOs.CommanDTOs;
using ProjectGroupService.DTOs;
using ProjectGroupService.Exceptions;
using ProjectGroupService.Repository.ProjectGroup;

namespace ProjectGroupService.Services.ProjectGroupServices;

public class ProjectGroupService(
    IProjectGroupRepository repository 
    /*, MicroserviceGateway gateway*/
) : IProjectGroupServices
{

    #region GET PROJECT GROUP PAGE
    public async Task<ListResult<ProjectGroupListDTO>> GetProjectGroupsPage()
    {
        var response = await repository.GetProjectGroupsPage();
        return response;
    }
    #endregion

    #region GET PROJECT GROUP VIEW
    public async Task<ProjectGroupViewDTO> GetProjectGroupView(int projectGroupID)
    {
        var response = await repository.GetProjectGroupView(projectGroupID)
                                        ?? throw new NotFoundException("Project Group not found");
        return response;
    }
    #endregion

    #region GET PROJECT GROUP PK
    public async Task<ProjectGroupUpdateDTO> GetProjectGroupPK(int projectGroupID)
    {
        var response = await repository.GetProjectGroupPK(projectGroupID)
                                        ?? throw new NotFoundException("Project Group not found");
        return response;
    }
    #endregion

    #region CREATE PROJECT GROUP
    public async Task<OperationResultDTO> CreateProjectGroup(ProjectGroupCreateDTO dto)
    {
        var response = await repository.CreateProjectGroup(dto);
        return response;
    }
    #endregion

    #region UPDATE PROJECT GROUP
    public async Task<OperationResultDTO> UpdateProjectGroup(ProjectGroupUpdateDTO dto)
    {
        var response = await repository.UpdateProjectGroup(dto)
                                            ?? throw new NotFoundException("Project Group not found");
        return response;
    }
    #endregion

    #region DEACTIVATE PROJECT GROUP
    public async Task<OperationResultDTO> DeactivateProjectGroup(int projectGroupID)
    {
        var response = await repository.DeactivateProjectGroup(projectGroupID)
                                            ?? throw new NotFoundException("Project Group not found");
        return response;
    }
    #endregion
}
