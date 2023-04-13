using Identity.Api.Models;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Identity.Api.Configuration
{
    public class MyResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public readonly SignInManager<ApplicationUser> signInManager;
        private readonly MyUserManager userManager;
        //public readonly IEventService service;
        public MyResourceOwnerPasswordValidator(MyUserManager userService, SignInManager<ApplicationUser> signInManager)//, IEventService service)
        {
            userManager = userService;
            this.signInManager = signInManager;
            //this.service = service;

        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if(string.IsNullOrEmpty(context.UserName) || string.IsNullOrEmpty(context.Password))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "验证被拒绝，用户名或者密码为空。");
                return;
            }

            var user = await userManager.FindByNameAsync(context.UserName);
            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "验证失败，不存在当前用户。");
                return;
            }

            //检验用户密码（虽然我也不知道他的密码是采用什么加密方式得到的，但是我也不需要知道） 
            var passwordPass = await userManager.CheckPasswordAsync(user, context.Password);
            if (!passwordPass)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "验证失败，用户凭证错误");
                return;
            }
            else
            {
                try
                {
                    await userManager.AddLoginAsync(user, new UserLoginInfo(user.Id, "", user.UserName));
                }
                catch (Exception ex)
                {
                    
                }
                finally
                {
                    context.Result = new GrantValidationResult(user.Id, GrantType.ResourceOwnerPassword, new List<Claim>() { new Claim("account", user.UserName) });
                }
            }
            return;

        }
    }
}
