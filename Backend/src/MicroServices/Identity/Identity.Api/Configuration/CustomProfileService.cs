using Identity.Api.Entities;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Identity.Api.Configuration
{
    public class CustomProfileService<TUser> : IProfileService
                where TUser : ApplicationUser

    {
        /// <summary>
        /// The claims factory.
        /// </summary>
        protected readonly IUserClaimsPrincipalFactory<TUser> ClaimsFactory;

        /// <summary>
        /// The user manager.
        /// </summary>
        protected readonly UserManager<TUser> UserManager;

        public CustomProfileService(
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            UserManager<TUser> userManager
            )
        {
            ClaimsFactory = claimsFactory;
            UserManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No sub claim present");

            var user = await UserManager.FindByIdAsync(sub);
            if (user == null)
            {
            }
            else
            {
                var calims = GetClaimsFromUserAsync(user);
                var principal = await ClaimsFactory.CreateAsync(user);
                if (principal == null) throw new Exception("ClaimsFactory failed to create a principal");

                //context.IssuedClaims = principal.Claims.ToList();
                calims.AddRange(principal.Claims);
                context.AddRequestedClaims(calims);
            }
        }

        public List<Claim> GetClaimsFromUserAsync(TUser user)
        {
            var claims = new List<Claim> {
                //new Claim(JwtClaimTypes.Subject,user.Id.ToString()),
                new Claim("username",user.Name??""),
                //new Claim("name",user.Name),
                new Claim("createdon",user.CreatedOn.ToString()),
                new Claim("picture",user.Picture??"")
            };

            //var role = await UserManager.GetRolesAsync(user);
            //role.ToList().ForEach(f =>
            //{
            //    claims.Add(new Claim(JwtClaimTypes.Role, f));
            //});

            return claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No subject Id claim present");

            var user = await UserManager.FindByIdAsync(sub);
            if (user == null)
            {
            }

            context.IsActive = user != null;
        }
    }
}