using FluentValidation;
using FluentValidation.AspNetCore;
using Identity.Api.Services.Account;

namespace Identity.Api.Middleware
{
    public static class FluentValidatationMiddleware
    {
        public static IServiceCollection AddCustomFluentValidatation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            services.AddScoped<IValidator<LoginInputModel>, LoginInputModelValidator>();

            return services;
        }
    }
}
