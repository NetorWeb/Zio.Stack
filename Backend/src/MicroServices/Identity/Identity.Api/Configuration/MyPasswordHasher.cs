using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using NETCore.Encrypt.Extensions;

namespace Identity.Api.Configuration
{
    public class MyPasswordHasher : PasswordHasher<ApplicationUser>
    {
        public override string HashPassword(ApplicationUser user, string password)
        {
            //PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            //var pstr = ph.HashPassword(new ApplicationUser(), password);
            //return pstr;
            return password.MD5();
        }

        public override PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
            if (providedPassword.MD5().Equals(hashedPassword))
            {
                return PasswordVerificationResult.Success;
            }
            else
            {
                return PasswordVerificationResult.Failed;
            }
        }
    }
}
