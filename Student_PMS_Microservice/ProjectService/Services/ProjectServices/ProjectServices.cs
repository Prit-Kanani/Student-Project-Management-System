using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using Comman.MicroserviceDTO;
using ProjectService.DTOs;
using ProjectService.Rpository.ProjectRepository;
using ProjectService.Services.External;

namespace ProjectService.Services.ProjectServices;

public class ProjectService(
    IProjectRepository repository,
    UserServiceClient userServiceClient,
    ILogger<ProjectService> logger
) : IProjectServices
{
    private readonly IProjectRepository _repository = repository;
    private readonly UserServiceClient _userServiceClient = userServiceClient;
    private readonly ILogger<ProjectService> _logger = logger;

    public async Task<ListResult<ProjectListDTO>> GetProjectsPage()
    {
        return await _repository.GetProjectsPage();
    }

    public async Task<ProjectViewDTO> GetProjectView(int projectID)
    {
        var response = await _repository.GetProjectView(projectID);

        try
        {
            var auditUsers = await _userServiceClient.ResolveAuditUsers(
                response.CreatedByID,
                response.ModifiedByID,
                null
            );

            response.CreatedBy = auditUsers.CreatedBy;
            response.ModifiedBy = auditUsers.ModifiedBy;
        }
        catch (Exception ex) when (
            ex is ApiException ||
            ex is HttpRequestException ||
            ex is TaskCanceledException
        )
        {
            _logger.LogWarning(
                ex,
                "Audit user resolution failed for project {ProjectID}. Returning the core project payload without audit names.",
                projectID
            );
        }

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
