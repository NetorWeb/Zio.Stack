﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Identity.Api.Middleware
{
    public static class AuthenticationMiddleware
    {
        public static void ConfigAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication()
             .AddJwtBearer("Bearer", options =>
             {
                 var configUrl = configuration.GetSection("IdentityServer4")["authUrls"];
                 options.Authority = configUrl;
                 options.RequireHttpsMetadata = false;
                 options.Audience = configuration.GetSection("IdentityServer4")["Audience"];
                 options.TokenValidationParameters.ValidateAudience = false;
             });
            //API授权
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "identityapi");
                });
            });
        }
    }
}
