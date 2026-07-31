using FluentValidation;

namespace GameVault.Source.Application.Feature.Auth.Commands.RevokeRefreshToken
{
    public sealed class RevokeRefreshTokenCommandValidator : AbstractValidator<RevokeRefreshTokenCommand>
    {
        public RevokeRefreshTokenCommandValidator() {

            RuleFor(x => x.userId)
                .NotEmpty().WithMessage("El id es obligatorio.");
     



        }

    }
    }

