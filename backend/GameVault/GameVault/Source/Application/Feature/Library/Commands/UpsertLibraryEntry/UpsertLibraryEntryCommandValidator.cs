using FluentValidation;

namespace GameVault.Source.Application.Feature.Library.Commands.UpsertLibraryEntry
{
    public sealed class UpsertLibraryEntryCommandValidator : AbstractValidator<UpsertLibraryEntryCommand>
    {
        public UpsertLibraryEntryCommandValidator()
        {
            RuleFor(x => x.ExternalId)
                .GreaterThan(0).WithMessage("El juego indicado no es válido.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("El estado indicado no es válido.");

            RuleFor(x => x.ProgressPercentage)
                .InclusiveBetween(0, 100).WithMessage("El progreso debe estar entre 0 y 100.");

            RuleFor(x => x.HoursPlayed)
                .InclusiveBetween(0, 100000).WithMessage("Las horas jugadas no son válidas.")
                .When(x => x.HoursPlayed is not null);

            RuleFor(x => x.PersonalRating)
                .InclusiveBetween(1, 10).WithMessage("La nota debe estar entre 1 y 10.")
                .When(x => x.PersonalRating is not null);

            RuleFor(x => x.CompletedAt)
                .GreaterThanOrEqualTo(x => x.StartedAt!.Value)
                .WithMessage("La fecha de fin no puede ser anterior a la de inicio.")
                .When(x => x.StartedAt is not null && x.CompletedAt is not null);

            RuleFor(x => x.PrivateNotes)
                .MaximumLength(2000).WithMessage("Las notas no pueden superar los 2000 caracteres.");
        }
    }
}
