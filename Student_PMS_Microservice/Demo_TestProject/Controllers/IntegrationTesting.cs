using Comman.DTOs.CommanDTOs;
using System.Net;
using System.Net.Http.Json;
using UserService.DTOs;
using UserService.Models;

namespace Demo_TestProject.Controllers;

public class IntegrationTesting
{
    private const string SkipReason = "Requires running services, seeded data, and environment-specific setup.";

    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://localhost:7095/api/UserService/User";
    private readonly UserData _userData = new();

    public IntegrationTesting()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
        _httpClient = new HttpClient(handler) { BaseAddress = new Uri(_baseUrl) };
    }

    private UserCreateDTO GetTestUserCreateDTO()
    {
        return new UserCreateDTO
        {
            Name = _userData.TempData.Name,
            Email = _userData.TempData.Email,
            Password = _userData.TempData.Password,
            IsActive = _userData.TempData.IsActive,
            RoleID = _userData.TempData.RoleID
        };
    }

    private UserUpdateDTO GetTestUserUpdateDTO(int userId)
    {
        return new UserUpdateDTO
        {
            UserID = userId,
            Name = _userData.TempData.Name + " Updated",
            Email = _userData.TempData.Email,
            IsActive = _userData.TempData.IsActive,
            RoleID = _userData.TempData.RoleID
        };
    }

    [Fact(Skip = SkipReason)]
    public async Task GetUsers_ShouldReturnOkAndListOfUsers()
    {
        var response = await _httpClient.GetAsync("/Page");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ListResult<UserListDTO>>();
        Assert.NotNull(content);
        Assert.NotNull(content.Items);
    }

    [Fact(Skip = SkipReason)]
    public async Task GetUsers_ResponseShouldHaveValidStructure()
    {
        var response = await _httpClient.GetAsync("/Page");
        var content = await response.Content.ReadFromJsonAsync<ListResult<UserListDTO>>();

        Assert.NotNull(content);
        Assert.NotNull(content.Items);
        Assert.IsType<List<UserListDTO>>(content.Items);
    }

    [Fact(Skip = SkipReason)]
    public async Task GetUserByPK_WithValidId_ShouldReturnOkAndUserData()
    {
        const int userId = 1;
        var response = await _httpClient.GetAsync($"/Pk/{userId}");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<UserUpdateDTO>();
        Assert.NotNull(content);
        Assert.Equal(userId, content.UserID);
    }

    [Fact(Skip = SkipReason)]
    public async Task GetUserByPK_WithInvalidId_ShouldReturnNotFound()
    {
        var response = await _httpClient.GetAsync("/Pk/99999");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(Skip = SkipReason)]
    public async Task ViewUser_WithValidId_ShouldReturnOkAndUserViewData()
    {
        var response = await _httpClient.GetAsync("/View/1");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<UserViewDTO>();
        Assert.NotNull(content);
    }

    [Fact(Skip = SkipReason)]
    public async Task ViewUser_ResponseShouldContainUserDetails()
    {
        var response = await _httpClient.GetAsync("/View/1");
        var content = await response.Content.ReadFromJsonAsync<UserViewDTO>();

        Assert.NotNull(content);
        Assert.False(string.IsNullOrWhiteSpace(content.Name));
        Assert.False(string.IsNullOrWhiteSpace(content.Email));
    }

    [Fact(Skip = SkipReason)]
    public async Task CreateUser_WithValidData_ShouldReturnOkAndOperationResult()
    {
        var userCreateDTO = new UserCreateDTO
        {
            Name = "Test User " + Guid.NewGuid(),
            Email = $"testuser{Guid.NewGuid()}@gmail.com",
            Password = "TestPassword@123",
            IsActive = true,
            RoleID = 1
        };

        var response = await _httpClient.PostAsJsonAsync("/Insert", userCreateDTO);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<OperationResultDTO>();
        Assert.NotNull(content);
        Assert.True(content.Id > 0);
        Assert.True(content.RowsAffected > 0);
    }

    [Fact(Skip = SkipReason)]
    public async Task CreateUser_WithTestData_ShouldCreateUserSuccessfully()
    {
        var testUserDTO = GetTestUserCreateDTO();
        testUserDTO.Email = $"john_doe_{Guid.NewGuid()}@gmail.com";

        var response = await _httpClient.PostAsJsonAsync("/Insert", testUserDTO);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<OperationResultDTO>();
        Assert.NotNull(content);
        Assert.True(content.Id > 0);
    }

    [Fact(Skip = SkipReason)]
    public async Task CreateUser_WithDuplicateEmail_ShouldFail()
    {
        var userDTO = new UserCreateDTO
        {
            Name = "Duplicate Test",
            Email = "duplicate@gmail.com",
            Password = "Password@123",
            IsActive = true,
            RoleID = 1
        };

        await _httpClient.PostAsJsonAsync("/Insert", userDTO);
        var duplicateResponse = await _httpClient.PostAsJsonAsync("/Insert", userDTO);

        Assert.NotNull(duplicateResponse);
        Assert.True(
            duplicateResponse.StatusCode == HttpStatusCode.BadRequest ||
            duplicateResponse.StatusCode == HttpStatusCode.Conflict ||
            duplicateResponse.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Theory(Skip = SkipReason)]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateUser_WithInvalidName_ShouldReturnBadRequest(string invalidName)
    {
        var userDTO = new UserCreateDTO
        {
            Name = invalidName,
            Email = "test@gmail.com",
            Password = "Password@123",
            IsActive = true,
            RoleID = 1
        };

        var response = await _httpClient.PostAsJsonAsync("/Insert", userDTO);

        Assert.NotNull(response);
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.UnprocessableEntity);
    }

    [Fact(Skip = SkipReason)]
    public async Task UpdateUser_WithValidData_ShouldReturnOkAndOperationResult()
    {
        var userUpdateDTO = GetTestUserUpdateDTO(1);
        var response = await _httpClient.PutAsJsonAsync("/Update", userUpdateDTO);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<OperationResultDTO>();
        Assert.NotNull(content);
        Assert.True(content.RowsAffected >= 0);
    }

    [Fact(Skip = SkipReason)]
    public async Task UpdateUser_WithNonExistentId_ShouldFail()
    {
        var userUpdateDTO = GetTestUserUpdateDTO(99999);
        var response = await _httpClient.PutAsJsonAsync("/Update", userUpdateDTO);

        Assert.NotNull(response);
        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact(Skip = SkipReason)]
    public async Task UpdateUser_ShouldUpdateUserName()
    {
        const int userId = 1;
        var newName = "Updated User Name " + Guid.NewGuid();
        var userUpdateDTO = GetTestUserUpdateDTO(userId);
        userUpdateDTO.Name = newName;

        var response = await _httpClient.PutAsJsonAsync("/Update", userUpdateDTO);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var getResponse = await _httpClient.GetAsync($"/Pk/{userId}");
        var updatedUser = await getResponse.Content.ReadFromJsonAsync<UserUpdateDTO>();
        Assert.NotNull(updatedUser);
        Assert.Equal(newName, updatedUser.Name);
    }

    [Fact(Skip = SkipReason)]
    public async Task DeactivateUser_WithValidId_ShouldReturnOkAndOperationResult()
    {
        var response = await _httpClient.DeleteAsync("/Deactivate/1");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<OperationResultDTO>();
        Assert.NotNull(content);
        Assert.True(content.RowsAffected >= 0);
    }

    [Fact(Skip = SkipReason)]
    public async Task DeactivateUser_WithNonExistentId_ShouldFail()
    {
        var response = await _httpClient.DeleteAsync("/Deactivate/99999");

        Assert.NotNull(response);
        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact(Skip = SkipReason)]
    public async Task DeactivateUser_ShouldSetUserAsInactive()
    {
        const int userId = 1;
        await _httpClient.DeleteAsync($"/Deactivate/{userId}");

        var getResponse = await _httpClient.GetAsync($"/Pk/{userId}");
        var deactivatedUser = await getResponse.Content.ReadFromJsonAsync<UserUpdateDTO>();
        Assert.NotNull(deactivatedUser);
        Assert.False(deactivatedUser.IsActive);
    }

    [Fact(Skip = SkipReason)]
    public async Task GetUser_WithInvalidEndpoint_ShouldReturnNotFound()
    {
        var response = await _httpClient.GetAsync("/InvalidEndpoint");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(Skip = SkipReason)]
    public async Task CreateUser_WithMissingRequiredFields_ShouldReturnValidationError()
    {
        var invalidUserDTO = new UserCreateDTO
        {
            Name = _userData.TempData.Name
        };

        var response = await _httpClient.PostAsJsonAsync("/Insert", invalidUserDTO);

        Assert.NotNull(response);
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public void TestData_ShouldHaveValidUserData()
    {
        var tempData = _userData.TempData;

        Assert.NotNull(tempData);
        Assert.Equal(1, tempData.UserID);
        Assert.Equal("John Doe", tempData.Name);
        Assert.Equal("kananiprit0@gmail.com", tempData.Email);
        Assert.NotEmpty(tempData.Password);
        Assert.True(tempData.IsActive);
        Assert.Equal(1, tempData.RoleID);
    }

    [Fact]
    public void TestData_UserDataCanBeUpdated()
    {
        var userData = new UserData();
        var newUser = new User
        {
            UserID = 2,
            Name = "Jane Doe",
            Email = "jane@gmail.com",
            Password = "password123",
            IsActive = true,
            RoleID = 2
        };

        userData.TempData = newUser;

        Assert.Equal(2, userData.TempData.UserID);
        Assert.Equal("Jane Doe", userData.TempData.Name);
        Assert.Equal("jane@gmail.com", userData.TempData.Email);
    }
}

public class UserData
{
    private User tempData = new()
    {
        UserID = 1,
        Name = "John Doe",
        Email = "kananiprit0@gmail.com",
        Password = "prit kanani 9@",
        IsActive = true,
        Created = DateTime.UtcNow,
        CreatedByID = 1,
        Modified = DateTime.UtcNow,
        ModifiedByID = 1,
        RoleID = 1
    };

    public User TempData { get => tempData; set => tempData = value; }
}
