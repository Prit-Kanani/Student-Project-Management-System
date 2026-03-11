using Comman.Exceptions;
using Comman.MicroserviceDTO;
using System.Net.Http.Json;

namespace ProjectGroupService.Services.External;

public class ProjectServiceClient(
    HttpClient httpClient
)
{
    public async Task EnsureProjectExists(int projectID)
    {
        var response = await httpClient.GetAsync($"api/ProjectService/Project/Exists/{projectID}");
        if (!response.IsSuccessStatusCode)
        {
            throw new BadRequestException("Failed to validate ProjectID from ProjectService");
        }

        var payload = await response.Content.ReadFromJsonAsync<EntityExistsDTO>()
            ?? throw new ApiServerException("ProjectService returned an empty existence response");

        if (!payload.Exists)
        {
            throw new NotFoundException("Project not found");
        }
    }
}
