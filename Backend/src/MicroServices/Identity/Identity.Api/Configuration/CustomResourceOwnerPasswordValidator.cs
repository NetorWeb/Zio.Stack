using Identity.Api.Entities;
using Identity.Api.Models.Identity;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static IdentityModel.OidcConstants;

namespace Identity.Api.Configuration
{
    public class CustomResourceOwnerPasswordValidator<TUser, TRole> : IResourceOwnerPasswordValidator
        where TUser : ApplicationUser
        where TRole : ApplicationRole
    {
        public readonly SignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly RoleManager<TRole> _roleManager;
        public readonly IEventService _events;

        public CustomResourceOwnerPasswordValidator(
            UserManager<TUser> userService,
            RoleManager<TRole> roleManager,
            SignInManager<TUser> signInManager,
            IEventService service)
        {
            _userManager = userService;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _events = service;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (string.IsNullOrEmpty(context.UserName) || string.IsNullOrEmpty(context.Password))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "验证被拒绝，用户名或者密码为空。");
                return;
            }

            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user == null)
            {
                await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "invalid username", interactive: false));
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "验证失败，不存在当前用户。");
                return;
            }

            //检验用户密码（虽然我也不知道他的密码是采用什么加密方式得到的，但是我也不需要知道）
            var result = await _signInManager.CheckPasswordSignInAsync(user, context.Password, true);
            if (result.Succeeded)
            {
                var sub = await _userManager.GetUserIdAsync(user);

                await _events.RaiseAsync(new UserLoginSuccessEvent(context.UserName, sub, context.UserName, interactive: false));

                var claims = new List<Claim>();
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var roleid in userRoles)
                {
                    var roles = await _roleManager.FindByNameAsync(roleid);
                    if (roles != null)
                    {
                        var roleClaim = await _roleManager.GetClaimsAsync(roles);
                        claims.AddRange(roleClaim.ToList().FindAll(c => c.Type == AuthorizeClaims.ProductFault || c.Type == AuthorizeClaims.Simulation || c.Type == AuthorizeClaims.PartDesin || c.Type == AuthorizeClaims.Config));
                    }
                }

                //添加用户声明.
                var userClaims = await _userManager.GetClaimsAsync(user);
                claims.AddRange(userClaims.ToList().FindAll(c => c.Type == AuthorizeClaims.ProductFault || c.Type == AuthorizeClaims.Simulation || c.Type == AuthorizeClaims.PartDesin || c.Type == AuthorizeClaims.Config));

                var data = new Dictionary<string, object>
                {
                    { "Data", new { user.Id, user.Name, claims = claims.Select(d => new { d.Type, d.Value }) } }
                };
                context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password, customResponse: data);
                return;
            }
            else if (result.IsLockedOut)
            {
                await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "locked out", interactive: false));
            }
            else if (result.IsNotAllowed)
            {
                await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "not allowed", interactive: false));
            }
            else
            {
                await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "invalid credentials", interactive: false));
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
        }
    }
}