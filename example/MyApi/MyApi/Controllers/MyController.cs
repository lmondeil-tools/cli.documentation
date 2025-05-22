namespace MyApi.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MyController : ControllerBase
{
    private readonly IMyService _myService;

    MyController(IMyService myService)
    {
        _myService = myService;
    }
}
