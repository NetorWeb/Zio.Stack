using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCollect.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class AppManagerController : ControllerBase
{
    [Route("test")]
    [HttpGet]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Test()
    {
        return await Task.FromResult(Ok("test"));
    }
}