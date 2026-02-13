using Comman.DTOs.CommanDTOs;
using ProjectService.DTOs;
using ProjectService.Exceptions;
using ProjectService.Rpository.ProjectRepository;

namespace ProjectService.Services.ProjectServices;

public class ProjectService : IProjectServices
{
    #region CONFIGURATION

    private readonly IProjectRepository _repository;
    //private readonly MicroserviceGateway _gateway;

    public ProjectService(IProjectRepository repository /*, MicroserviceGateway gateway*/)
    {  
        _repository = repository;
        //_gatway = gatway;
    }

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
        if (response == null) throw new NotFoundException("Project not found");
        return response;
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
