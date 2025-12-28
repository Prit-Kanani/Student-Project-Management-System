using Comman.DTOs.CommanDTOs;
using ProjectGroupService.DTOs;

namespace ProjectGroupService.Services.ProjectGroupByProject;

public interface IProjectGroupByProjectService
{
    Task<ListResult<ProjectGroupListDTO>>   GetProjectGroupByProjectsPage();
    Task<ProjectGroupByProjectViewDTO>      GetProjectGroupByProjectView(int projectGroupByProjectID);
    Task<ProjectGroupByProjectUpdateDTO>    GetProjectGroupByProjectPK(int projectGroupByProjectID);
    Task<OperationResultDTO>                CreateProjectGroupByProject(ProjectGroupCreateDTO dto);
    Task<OperationResultDTO>                UpdateProjectGroupByProject(ProjectGroupUpdateDTO dto);
    Task<OperationResultDTO>                DeactivateProjectGroupByProject(int projectGroupByProjectID);
}
