using FluentValidation;

namespace GameVault.Source.Application.Feature.Reviews.Commands.CreateReview
{
    public sealed class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.ExternalId)
                .GreaterThan(0).WithMessage("El juego indicado no es válido.");

            Include(new ReviewContentRules<CreateReviewCommand>(
                x => x.Title, x => x.Content, x => x.Rating));
        }
    }

    /// <summary>Reglas compartidas por crear y editar una reseña.</summary>
    public sealed class ReviewContentRules<T> : AbstractValidator<T>
    {
        public ReviewContentRules(
            System.Linq.Expressions.Expression<Func<T, string>> title,
            System.Linq.Expressions.Expression<Func<T, string>> content,
            System.Linq.Expressions.Expression<Func<T, int>> rating)
        {
            RuleFor(title)
                .MaximumLength(120).WithMessage("El título no puede superar los 120 caracteres.");

            RuleFor(content)
                .NotEmpty().WithMessage("Escribe tu reseña.")
                .MaximumLength(5000).WithMessage("La reseña no puede superar los 5000 caracteres.");

            RuleFor(rating)
                .InclusiveBetween(1, 10).WithMessage("La nota debe estar entre 1 y 10.");
        }
    }
}
