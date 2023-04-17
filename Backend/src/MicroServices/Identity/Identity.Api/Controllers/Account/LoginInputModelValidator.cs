using FluentValidation;

namespace Identity.Api.Services.Account
{
    public class LoginInputModelValidator : AbstractValidator<LoginInputModel>
    {
        public LoginInputModelValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("UserName is Empty");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is Empty");
        }
    }
}
