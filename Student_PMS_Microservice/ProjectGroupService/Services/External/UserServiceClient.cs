using Comman.Exceptions;
using Comman.MicroserviceDTO;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace ProjectGroupService.Services.External;

public class UserServiceClient(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor
)
{
    public async Task<AuditUsersDTO> ResolveAuditUsers(int createdByID, int? modifiedByID, int? approvedByID)
    {
        var uri = $"api/UserService/User/ResolveAuditUsers?createdByID={createdByID}&modifiedByID={modifiedByID}&approvedByID={approvedByID}";
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        AttachAuthorizationHeader(request);

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new BadRequestException("Failed to resolve audit users from UserService");
        }

        return await response.Content.ReadFromJsonAsync<AuditUsersDTO>()
            ?? throw new ApiServerException("UserService returned an empty audit user response");
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
