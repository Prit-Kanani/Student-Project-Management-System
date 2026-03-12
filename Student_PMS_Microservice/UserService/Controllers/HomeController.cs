using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers;

public class HomeController : Controller
{
    public IActionResult Add(int a, int b)
    {
        if (a < 0 || b < 0)
            return BadRequest("The Number is lessthen 0");
        //
        return Ok(a + b);
    }
}
