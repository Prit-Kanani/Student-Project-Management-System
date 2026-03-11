using Comman.Functions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections;
using UserService.Controllers;
using UserService.DTOs;
using UserService.Repository.Auth;
using UserService.Services.Auth;

namespace Demo_TestProject.Controllers;

public class TestingController
{
    [Theory]
    [ClassData(typeof(SampleData))]
    public void Addmethod(int a, int b, object expected)
    {
        var controller = new HomeController();
        var result = controller.Add(a, b);
        Assert.NotNull(result);

        if (result is ObjectResult objectResult)
        {
            Assert.Equal(expected, objectResult.Value);
        }
        else
        {
            Assert.Equal(expected, result);
        }
    }

    [Theory]
    [ClassData(typeof(LoginData))]
    public async Task AuthCheck(string email, string password)
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:SecretKey", "SPMS_UserService_SecretKey!" },
            { "Jwt:Issuer", "SPMS_UserService" },
            { "Jwt:Audience", "SPMS_UserService" },
            { "Jwt:ExpiryMinutes", "60" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var authRepo = new Mock<IAuthRepository>();

        authRepo.Setup(x => x.Login(It.IsAny<string>()))
            .ReturnsAsync((string requestedEmail) => new LoginDTO
            {
                Email = requestedEmail,
                Password = HashPass.HashPassword(password)
            });

        authRepo.Setup(x => x.UserInfo(It.IsAny<string>()))
            .ReturnsAsync(new UserInfoDTO
            {
                UserID = 1,
                Email = email,
                UserName = "Prit kanani",
                RoleName = "Admin"
            });

        var authService = new AuthService(configuration, authRepo.Object);

        var loginResult = await authService.Login(new LoginDTO { Email = email, Password = password });

        Assert.NotNull(loginResult);
        Assert.NotNull(loginResult.Message);
        Assert.NotNull(loginResult.Token);
    }
}

public class SampleData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1, 2, 3 };
        yield return new object[] { 1, -2, "The Number is lessthen 0" };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class LoginData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "Kananiprit@gmail.com", "prit kanani 9@" };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
