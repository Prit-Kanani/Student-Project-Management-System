namespace ProjectGroup.Services.UserService;

public class UserMicroservice
{
    #region CONFIGURATION
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    public UserMicroservice(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }
    #endregion
}
