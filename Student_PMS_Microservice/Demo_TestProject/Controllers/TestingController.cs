using Microsoft.AspNetCore.Mvc;
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

    [Fact]
    public async Task AuthCheck()
    {
        Mock<IAuthRepository> authService = new();
        authService.Setup(x => x.Login(It.IsAny<string>()))
                   .ReturnsAsync((string email) => new LoginDTO { Email = email, Password = "mocked-password" });

        authService.Setup(x => x.UserInfo(It.IsAny<string>()))
                   .ReturnsAsync(new UserInfoDTO { Email = "" });

        // use authService.Object where needed

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
        yield return new object[] { "", "" };
        yield return new object[] { "", "" };
        yield return new object[] { "", "" };
        yield return new object[] { "", "" };
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}