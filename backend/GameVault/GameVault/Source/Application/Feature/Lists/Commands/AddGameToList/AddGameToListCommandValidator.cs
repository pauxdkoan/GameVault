using FluentValidation;

namespace GameVault.Source.Application.Feature.Lists.Commands.AddGameToList
{
    public sealed class AddGameToListCommandValidator : AbstractValidator<AddGameToListCommand>
    {
        public AddGameToListCommandValidator()
        {
            RuleFor(x => x.ExternalId)
                .GreaterThan(0).WithMessage("El juego indicado no es válido.");

            RuleFor(x => x.Note)
                .MaximumLength(300).WithMessage("La nota no puede superar los 300 caracteres.");
        }
    }
}
