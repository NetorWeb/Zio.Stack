using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers.Account;

public class ExternalController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}