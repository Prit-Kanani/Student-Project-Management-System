namespace ProjectService.Services.ProjectServices;

public class ProjectMicroservices(
    HttpClient httpClient,
    IConfiguration config
)
{
    #region CONFIGURATION
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;
    #endregion
}
