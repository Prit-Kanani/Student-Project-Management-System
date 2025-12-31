using Comman.DTOs.CommanDTOs;
using ProjectGroupService.DTOs;

namespace ProjectGroupService.Repository.ProjectGroup;

public interface IProjectGroupRepository
{
    Task<ListResult<ProjectGroupListDTO>>   GetProjectGroupsPage();
    Task<ProjectGroupViewDTO>               GetProjectGroupView(int projectGroupID);
    Task<ProjectGroupUpdateDTO>             GetProjectGroupPK(int projectGroupID);
    Task<OperationResultDTO>                CreateProjectGroup(ProjectGroupCreateDTO dto);
    Task<OperationResultDTO>                UpdateProjectGroup(ProjectGroupUpdateDTO dto);
    Task<OperationResultDTO>                DeactivateProjectGroup(int projectGroupID);
}
