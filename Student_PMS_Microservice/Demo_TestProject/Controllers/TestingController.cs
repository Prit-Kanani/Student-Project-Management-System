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

public class TestingController(
)
{

    [Theory]
    [ClassData(typeof(SampleData))]
    public void Addmethod(int a, int b, object expected)
    {
        var controller = new HomeController();
        var result = controller.Add(a, b);
        Assert.NotNull(result);
        // If the controller returns an ObjectResult (OkObjectResult/BadRequestObjectResult), compare the Value
        if (result is ObjectResult objectResult)
        {
            Assert.Equal(expected, objectResult.Value);
        }
        else
        {
            // Fallback: compare the IActionResult directly (for non-ObjectResult implementations)
            Assert.Equal(expected, result);
        }
    }

    [Theory]
    [ClassData(typeof(LoginData))]
    public async Task AuthCheck(string Email, string Password)
    {
        //Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "SectionName:KeyName", "Value" },
            { "OtherKey", "OtherValue" }
        };

        IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Mock<IAuthRepository> authRepo = new();

        authRepo.Setup(x => x.Login(It.IsAny<string>()))
                   .ReturnsAsync((string email) => new LoginDTO { Email = email, Password = HashPass.HashPassword(Password) });

        authRepo.Setup(x => x.UserInfo(It.IsAny<string>()))
                   .ReturnsAsync(new UserInfoDTO { UserID = 1, Email = Email, UserName = "Prit kanani" ,RoleName = "Admmin"  });

        var authService= new AuthService(_configuration, authRepo.Object);


        //Act
        var loginResult = await authService.Login(new LoginDTO { Email= Email, Password=Password});

        // Assert
            
        Assert.NotNull(loginResult);
        Assert.NotNull(loginResult.Message);
        Assert.NotNull(loginResult.Token);        
    }
}

#region FackDatas
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
        yield return new object[] { "Kananiprit@gmail.com", "prit kanani 9@"};
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
#endregion