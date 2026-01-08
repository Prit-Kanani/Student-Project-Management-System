using Comman.DTOs.CommanDTOs;
using ProjectGroupService.DTOs;

namespace ProjectGroupService.Services.ProjectGroupByProject;

public interface IProjectGroupByProjectService
{
    Task<ListResult<ProjectGroupByProjectListDTO>>      GetProjectGroupByProjectsPage();
    Task<ProjectGroupByProjectViewDTO>                  GetProjectGroupByProjectView(int projectGroupByProjectID);
    Task<ProjectGroupByProjectUpdateDTO>                GetProjectGroupByProjectPK(int projectGroupByProjectID);
    Task<OperationResultDTO>                            CreateProjectGroupByProject(ProjectGroupByProjectCreateDTO dto);
    Task<OperationResultDTO>                            UpdateProjectGroupByProject(ProjectGroupByProjectUpdateDTO dto);
    Task<OperationResultDTO>                            DeactivateProjectGroupByProject(int projectGroupByProjectID);
    Task                                                BulkInsertAsync(List<BulkProjectGroupByProjectCreateDTO> projectGroupByProjects);
}
