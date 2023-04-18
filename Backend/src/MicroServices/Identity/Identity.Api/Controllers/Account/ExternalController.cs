using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;

namespace Identity.Api.Controllers.Account;

[Route("[controller]/[action]")]
public class ExternalController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;
    
    public ExternalController(
        IIdentityServerInteractionService interaction
        )
    {
        _interaction = interaction;
    }
    
    [HttpGet]
    public IActionResult Challenge(string scheme, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

        // validate returnUrl - either it is a valid OIDC URL or back to a local page
        if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
        {
            // user might have clicked on a malicious link - should be logged
            throw new Exception("invalid return URL");
        }

        // start challenge and roundtrip the return URL and scheme 
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(Callback)),
            Items =
            {
                { "returnUrl", returnUrl },
                { "scheme", scheme },
            }
        };

        return Challenge(props, scheme);
    }

    [HttpGet]
    public IActionResult Callback()
    {
        return Redirect("~/");
    }
}