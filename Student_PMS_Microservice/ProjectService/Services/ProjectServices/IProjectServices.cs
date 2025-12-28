using Comman.DTOs.CommanDTOs;
using ProjectService.DTOs;

namespace ProjectService.Services.ProjectServices;

public interface IProjectServices
{
    Task<ListResult<ProjectListDTO>>    GetProjectsPage();
    Task<ProjectViewDTO>                GetProjectView(int projectID);
    Task<ProjectUpdateDTO>              GetProjectPK(int projectID);
    Task<OperationResultDTO>            CreateProject(ProjectCreateDTO dto);
    Task<OperationResultDTO>            UpdateProject(ProjectUpdateDTO dto);
    Task<OperationResultDTO>            DeactivateProject(int projectID);
}
