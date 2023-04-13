using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Api.Configuration
{
    public class MyUserManager : UserManager<ApplicationUser>
    {
        public MyUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            optionsAccessor.Value.Password.RequireDigit = false;
            optionsAccessor.Value.Password.RequiredLength = 4;
            optionsAccessor.Value.Password.RequireLowercase = false;
            optionsAccessor.Value.Password.RequireUppercase = false;
            optionsAccessor.Value.Password.RequireNonAlphanumeric = false;
        }
    }
}
