using Comman.Exceptions;
using Comman.MicroserviceDTO;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace ProjectGroupService.Services.External;

public class ProjectServiceClient(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor
)
{
    public async Task EnsureProjectExists(int projectID)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/ProjectService/Project/Exists/{projectID}");
        AttachAuthorizationHeader(request);

        var response = await httpClient.SendAsync(request);
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

    private void AttachAuthorizationHeader(HttpRequestMessage request)
    {
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authorizationHeader);
        }
    }
}
