using Comman.DTOs.CommanDTOs;
using ProjectService.DTOs;
using ProjectService.Exceptions;
using ProjectService.Rpository.ProjectRepository;

namespace ProjectService.Services.ProjectServices;

public class ProjectService(
    IProjectRepository repository 
    /*, MicroserviceGateway gateway*/
) : IProjectServices
{
    #region CONFIGURATION

    private readonly IProjectRepository _repository = repository;

    #endregion

    #region GET PROJECT PAGE
    public async Task<ListResult<ProjectListDTO>> GetProjectsPage()
    {
        var response = await _repository.GetProjectsPage();
        return response;
    }
    #endregion

    #region GET PROJECT VIEW
    public async Task<ProjectViewDTO> GetProjectView(int projectID)
    {
        var response = await _repository.GetProjectView(projectID);
        return response ?? throw new NotFoundException("Project not found");
    }
    #endregion

    #region GET PROJECT PK
    public async Task<ProjectUpdateDTO> GetProjectPK(int projectID)
    {
        var response = await _repository.GetProjectPK(projectID)
                                        ?? throw new NotFoundException("Project not found");
        return response;
    }
    #endregion

    #region CREATE PROJECT
    public async Task<OperationResultDTO> CreateProject(ProjectCreateDTO dto)
    {
        var response = await _repository.CreateProject(dto);
        return response;
    }
    #endregion

    #region UPDATE PROJECT
    public async Task<OperationResultDTO> UpdateProject(ProjectUpdateDTO dto)
    {
        var response = await _repository.UpdateProject(dto)
                                            ?? throw new NotFoundException("Project not found");
        return response;
    }
    #endregion

    #region DEACTIVATE USER
    public async Task<OperationResultDTO> DeactivateProject(int projectID)
    {
        var response = await _repository.DeactivateProject(projectID)
                                            ?? throw new NotFoundException("Project not found");
        return response;
    }
    #endregion
}
