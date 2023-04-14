using FluentValidation;
using Identity.Api.Entities;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Services.Account;

[Authorize]
public class AccountService : ServiceBase
{
    private SignInManager<ApplicationUser> _signInManager => GetRequiredService<SignInManager<ApplicationUser>>();
    private UserManager<ApplicationUser> _userManager => GetRequiredService<UserManager<ApplicationUser>>();
    private IEventService _events => GetRequiredService<IEventService>();
    private IIdentityServerInteractionService _interaction => GetRequiredService<IIdentityServerInteractionService>();

    [AllowAnonymous]
    [RoutePattern("/Account/Login",HttpMethod = "Get")]
    public async Task<IResult> Login([FromQuery]string returnUrl)
    {
        //await Login(new LoginInputModel { Username = "administrator", Password = "admin@123...", RememberLogin = true, ReturnUrl = returnUrl });

        return Results.Ok();
    }

    [AllowAnonymous]
    [RoutePattern(HttpMethod = "Post")]
    public async Task<IResult> Login([FromServices]IValidator<LoginInputModel> validator, [FromBody] LoginInputModel model)
    {
        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));
        }
        else
        {
            await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
        }

        return Results.Ok();
    }

    [RoutePattern(HttpMethod = "Get")]
    public Task<IResult> Auth()
    {
        return Task.FromResult(Results.Ok());
    }
}