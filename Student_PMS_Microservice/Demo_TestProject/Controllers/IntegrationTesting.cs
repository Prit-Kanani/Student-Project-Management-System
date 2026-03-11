using Comman.DTOs.CommanDTOs;
using System.Net;
using System.Net.Http.Json;
using UserService.DTOs;
using UserService.Models;

namespace Demo_TestProject.Controllers;

public class IntegrationTesting
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://localhost:5001/api/UserService/User";

    public IntegrationTesting()
    {
        // Note: This assumes the UserService is running on localhost:5001
        // For proper integration tests, use WebApplicationFactory<Program> or similar
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        _httpClient = new HttpClient(handler) { BaseAddress = new Uri(_baseUrl) };
    }

    #region TEST DATA SETUP
    private readonly UserData _userData = new();

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
    #endregion

    #region GET USERS PAGE TEST
    [Fact]
    public async Task GetUsers_ShouldReturnOkAndListOfUsers()
    {
        // Arrange
        var endpoint = "/Page";

        // Act
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsAsync<ListResult<UserListDTO>>();
        Assert.NotNull(content);
        Assert.NotNull(content.Data);
    }

    [Fact]
    public async Task GetUsers_ResponseShouldHaveValidStructure()
    {
        // Arrange
        var endpoint = "/Page";

        // Act
        var response = await _httpClient.GetAsync(endpoint);
        var content = await response.Content.ReadAsAsync<ListResult<UserListDTO>>();

        // Assert
        Assert.NotNull(content.Data);
        Assert.IsType<List<UserListDTO>>(content.Data);
    }
    #endregion

    #region GET USER BY PRIMARY KEY TEST
    [Fact]
    public async Task GetUserByPK_WithValidId_ShouldReturnOkAndUserData()
    {
        // Arrange
        int userId = 1; // Assuming user with ID 1 exists
        var endpoint = $"/Pk/{userId}";

        // Act
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsAsync<UserUpdateDTO>();
        Assert.NotNull(content);
        Assert.Equal(userId, content.UserID);
    }

    [Fact]
    public async Task GetUserByPK_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        int invalidUserId = 99999;
        var endpoint = $"/Pk/{invalidUserId}";

        // Act
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region VIEW USER TEST
    [Fact]
    public async Task ViewUser_WithValidId_ShouldReturnOkAndUserViewData()
    {
        // Arrange
        int userId = 1; // Assuming user with ID 1 exists
        var endpoint = $"/View/{userId}";

        // Act
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsAsync<UserViewDTO>();
        Assert.NotNull(content);
    }

    [Fact]
    public async Task ViewUser_ResponseShouldContainUserDetails()
    {
        // Arrange
        int userId = 1;
        var endpoint = $"/View/{userId}";

        // Act
        var response = await _httpClient.GetAsync(endpoint);
        var content = await response.Content.ReadAsAsync<UserViewDTO>();

        // Assert
        Assert.NotNull(content.Name);
        Assert.NotNull(content.Email);
        Assert.False(string.IsNullOrWhiteSpace(content.Name));
    }
    #endregion

    #region CREATE USER TEST
    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnOkAndSuccessMessage()
    {
        // Arrange
        var userCreateDTO = new UserCreateDTO
        {
            Name = "Test User " + Guid.NewGuid(),
            Email = $"testuser{Guid.NewGuid()}@gmail.com",
            Password = "TestPassword@123",
            IsActive = true,
            RoleID = 1
        };
        var endpoint = "/Insert";

        // Act
        var response = await _httpClient.PostAsJsonAsync(endpoint, userCreateDTO);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsAsync<OperationResultDTO>();
        Assert.NotNull(content);
        Assert.True(content.Success, "User creation should be successful");
    }

    [Fact]
    public async Task CreateUser_WithTestData_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var testUserDTO = GetTestUserCreateDTO();
        testUserDTO.Email = $"john_doe_{Guid.NewGuid()}@gmail.com"; // Ensure unique email
        var endpoint = "/Insert";

        // Act
        var response = await _httpClient.PostAsJsonAsync(endpoint, testUserDTO);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsAsync<OperationResultDTO>();
        Assert.True(content.Success);
        Assert.NotNull(content.Message);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail_ShouldFail()
    {
        // Arrange
        string duplicateEmail = "duplicate@gmail.com";
        var userDTO = new UserCreateDTO
        {
            Name = "Duplicate Test",
            Email = duplicateEmail,
            Password = "Password@123",
            IsActive = true,
            RoleID = 1
        };
        var endpoint = "/Insert";

        // Act - First user creation
        await _httpClient.PostAsJsonAsync(endpoint, userDTO);

        // Act - Duplicate user creation
        var duplicateResponse = await _httpClient.PostAsJsonAsync(endpoint, userDTO);

        // Assert
        Assert.NotNull(duplicateResponse);
        Assert.False((await duplicateResponse.Content.ReadAsAsync<OperationResultDTO>()).Success,
            "Duplicate email should fail");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateUser_WithInvalidName_ShouldReturnBadRequest(string invalidName)
    {
        // Arrange
        var userDTO = new UserCreateDTO
        {
            Name = invalidName,
            Email = "test@gmail.com",
            Password = "Password@123",
            IsActive = true,
            RoleID = 1
        };
        var endpoint = "/Insert";

        // Act
        var response = await _httpClient.PostAsJsonAsync(endpoint, userDTO);

        // Assert
        Assert.NotNull(response);
        // Expect either BadRequest or UnprocessableEntity
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest ||
                   response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity);
    }
    #endregion

    #region UPDATE USER TEST
    [Fact]
    public async Task UpdateUser_WithValidData_ShouldReturnOkAndSuccessMessage()
    {
        // Arrange
        int existingUserId = 1; // Assuming user with ID 1 exists
        var userUpdateDTO = GetTestUserUpdateDTO(existingUserId);
        var endpoint = "/Update";

        // Act
        var response = await _httpClient.PutAsJsonAsync(endpoint, userUpdateDTO);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsAsync<OperationResultDTO>();
        Assert.NotNull(content);
        Assert.True(content.Success, "User update should be successful");
    }

    [Fact]
    public async Task UpdateUser_WithNonExistentId_ShouldFail()
    {
        // Arrange
        var userUpdateDTO = GetTestUserUpdateDTO(99999);
        var endpoint = "/Update";

        // Act
        var response = await _httpClient.PutAsJsonAsync(endpoint, userUpdateDTO);

        // Assert
        Assert.NotNull(response);
        var content = await response.Content.ReadAsAsync<OperationResultDTO>();
        Assert.False(content.Success, "Update of non-existent user should fail");
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUserName()
    {
        // Arrange
        int userId = 1;
        var newName = "Updated User Name " + Guid.NewGuid();
        var userUpdateDTO = GetTestUserUpdateDTO(userId);
        userUpdateDTO.Name = newName;
        var endpoint = "/Update";

        // Act
        var response = await _httpClient.PutAsJsonAsync(endpoint, userUpdateDTO);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updateResult = await response.Content.ReadAsAsync<OperationResultDTO>();
        Assert.True(updateResult.Success);

        // Verify update by fetching the user
        var getResponse = await _httpClient.GetAsync($"/Pk/{userId}");
        var updatedUser = await getResponse.Content.ReadAsAsync<UserUpdateDTO>();
        Assert.Equal(newName, updatedUser.Name);
    }
    #endregion

    #region DEACTIVATE USER TEST
    [Fact]
    public async Task DeactivateUser_WithValidId_ShouldReturnOkAndSuccessMessage()
    {
        // Arrange
        int userId = 1; // Assuming user with ID 1 exists
        var endpoint = $"/Deactivate/{userId}";

        // Act
        var response = await _httpClient.DeleteAsync(endpoint);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsAsync<OperationResultDTO>();
        Assert.NotNull(content);
        Assert.True(content.Success, "User deactivation should be successful");
    }

    [Fact]
    public async Task DeactivateUser_WithNonExistentId_ShouldFail()
    {
        // Arrange
        int invalidUserId = 99999;
        var endpoint = $"/Deactivate/{invalidUserId}";

        // Act
        var response = await _httpClient.DeleteAsync(endpoint);

        // Assert
        Assert.NotNull(response);
        var content = await response.Content.ReadAsAsync<OperationResultDTO>();
        Assert.False(content.Success, "Deactivation of non-existent user should fail");
    }

    [Fact]
    public async Task DeactivateUser_ShouldSetUserAsInactive()
    {
        // Arrange
        int userId = 1;
        var deactivateEndpoint = $"/Deactivate/{userId}";

        // Act
        await _httpClient.DeleteAsync(deactivateEndpoint);

        // Assert - Verify user is inactive by fetching
        var getResponse = await _httpClient.GetAsync($"/Pk/{userId}");
        var deactivatedUser = await getResponse.Content.ReadAsAsync<UserUpdateDTO>();
        Assert.False(deactivatedUser.IsActive, "User should be deactivated");
    }
    #endregion

    #region ERROR HANDLING TESTS
    [Fact]
    public async Task GetUser_WithInvalidEndpoint_ShouldReturnNotFound()
    {
        // Arrange
        var invalidEndpoint = "/InvalidEndpoint";

        // Act
        var response = await _httpClient.GetAsync(invalidEndpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithMissingRequiredFields_ShouldReturnValidationError()
    {
        // Arrange
        var invalidUserDTO = new UserCreateDTO
        {
            Name = _userData.TempData.Name
            // Missing Email and Password
        };
        var endpoint = "/Insert";

        // Act
        var response = await _httpClient.PostAsJsonAsync(endpoint, invalidUserDTO);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest ||
                   response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity,
            "Missing required fields should return validation error");
    }
    #endregion

    #region FACT DATA VALIDATION TESTS
    [Fact]
    public void TestData_ShouldHaveValidUserData()
    {
        // Arrange & Act
        var tempData = _userData.TempData;

        // Assert
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
        // Arrange
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

        // Act
        userData.TempData = newUser;

        // Assert
        Assert.Equal(2, userData.TempData.UserID);
        Assert.Equal("Jane Doe", userData.TempData.Name);
        Assert.Equal("jane@gmail.com", userData.TempData.Email);
    }
    #endregion
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