using FluentValidation;

namespace GameVault.Source.Application.Feature.Lists.Commands.CreateList
{
    public sealed class CreateListCommandValidator : AbstractValidator<CreateListCommand>
    {
        public CreateListCommandValidator()
        {
            Include(new ListContentRules<CreateListCommand>(x => x.Name, x => x.Description));
        }
    }

    /// <summary>Reglas compartidas por crear y editar una lista.</summary>
    public sealed class ListContentRules<T> : AbstractValidator<T>
    {
        public ListContentRules(
            System.Linq.Expressions.Expression<Func<T, string>> name,
            System.Linq.Expressions.Expression<Func<T, string?>> description)
        {
            RuleFor(name)
                .NotEmpty().WithMessage("Ponle un nombre a la lista.")
                .MaximumLength(80).WithMessage("El nombre no puede superar los 80 caracteres.");

            RuleFor(description)
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");
        }
    }
}
