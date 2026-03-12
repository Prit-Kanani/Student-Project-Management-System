using Comman.DTOs.CommanDTOs;
using Comman.MicroserviceDTO;
using ProjectService.DTOs;
using ProjectService.Rpository.ProjectRepository;
using ProjectService.Services.External;

namespace ProjectService.Services.ProjectServices;

public class ProjectService(
    IProjectRepository repository,
    UserServiceClient userServiceClient
) : IProjectServices
{
    private readonly IProjectRepository _repository = repository;
    private readonly UserServiceClient _userServiceClient = userServiceClient;

    public async Task<ListResult<ProjectListDTO>> GetProjectsPage()
    {
        return await _repository.GetProjectsPage();
    }

    public async Task<ProjectViewDTO> GetProjectView(int projectID)
    {
        var response = await _repository.GetProjectView(projectID);
        var auditUsers = await _userServiceClient.ResolveAuditUsers(response.CreatedByID, response.ModifiedByID, null);
        response.CreatedBy = auditUsers.CreatedBy;
        response.ModifiedBy = auditUsers.ModifiedBy;
        return response;
    }

    public async Task<ProjectUpdateDTO> GetProjectPK(int projectID)
    {
        return await _repository.GetProjectPK(projectID);
    }

    public async Task<OperationResultDTO> CreateProject(ProjectCreateDTO dto)
    {
        return await _repository.CreateProject(dto);
    }

    public async Task<OperationResultDTO> UpdateProject(ProjectUpdateDTO dto)
    {
        return await _repository.UpdateProject(dto);
    }

    public async Task<OperationResultDTO> DeactivateProject(int projectID)
    {
        return await _repository.DeactivateProject(projectID);
    }

    public async Task<EntityExistsDTO> ProjectExists(int projectID)
    {
        return new EntityExistsDTO
        {
            Id = projectID,
            Exists = await _repository.ProjectExists(projectID)
        };
    }
}
