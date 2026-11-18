using FluentValidation;

namespace GameVault.Source.Application.Feature.Auth.Queries.Login
{
    public sealed class LoginWithRefreshTokenQueryValidator : AbstractValidator<LoginWithRefreshTokenQuery>
    {
        public LoginWithRefreshTokenQueryValidator() {


            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("El refresh token es obligatorio");      

        }

    }
 }

