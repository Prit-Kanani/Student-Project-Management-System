using Comman.DTOs.CommanDTOs;
using ProjectGroupService.DTOs;

namespace ProjectGroupService.Rpository.ProjectGroupByProject;

public interface IProjectGroupByProjectRepository
{
    Task<ListResult<ProjectGroupByProjectListDTO>> GetProjectGroupByProjectsPage();
    Task<ProjectGroupByProjectViewDTO> GetProjectGroupByProjectView(int projectGroupByProjectID);
    Task<ProjectGroupByProjectUpdateDTO> GetProjectGroupByProjectPK(int projectGroupByProjectID);
    Task<OperationResultDTO> CreateProjectGroupByProject(ProjectGroupByProjectCreateDTO dto);
    Task<OperationResultDTO> UpdateProjectGroupByProject(ProjectGroupByProjectUpdateDTO dto);
    Task<OperationResultDTO> DeactivateProjectGroupByProject(int projectGroupByProjectID);
}
