using FluentValidation;

namespace GameVault.Source.Application.Feature.Auth.Commands.RegisterUser
{
    public sealed class RegisterUserCommandValidator:AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator() { 
            
            RuleFor(x=>x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.UserName)
           .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
           .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres.")
           .MaximumLength(30).WithMessage("El nombre de usuario no puede superar los 30 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es obligatorio.")
                .EmailAddress().WithMessage("El correo no tiene un formato válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número.");

     



        }

    }
    }

