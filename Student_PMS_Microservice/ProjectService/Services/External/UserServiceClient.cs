using Comman.Exceptions;
using Comman.MicroserviceDTO;
using System.Net.Http.Json;

namespace ProjectService.Services.External;

public class UserServiceClient(
    HttpClient httpClient
)
{
    public async Task<AuditUsersDTO> ResolveAuditUsers(int createdByID, int? modifiedByID, int? approvedByID)
    {
        var uri = $"api/UserService/User/ResolveAuditUsers?createdByID={createdByID}&modifiedByID={modifiedByID}&approvedByID={approvedByID}";
        var response = await httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            throw new BadRequestException("Failed to resolve audit users from UserService");
        }

        return await response.Content.ReadFromJsonAsync<AuditUsersDTO>()
            ?? throw new ApiServerException("UserService returned an empty audit user response");
    }
}
