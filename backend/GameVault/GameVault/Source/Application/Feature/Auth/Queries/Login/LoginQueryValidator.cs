using FluentValidation;

namespace GameVault.Source.Application.Feature.Auth.Queries.Login
{
    public sealed class LoginQueryValidator : AbstractValidator<LoginQuery>
    {
        public LoginQueryValidator() { 
            
       
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es obligatorio.")
                .EmailAddress().WithMessage("El correo no tiene un formato válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.");
         
                

        }

    }
 }

