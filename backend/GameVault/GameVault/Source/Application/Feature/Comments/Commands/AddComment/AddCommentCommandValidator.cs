using FluentValidation;

namespace GameVault.Source.Application.Feature.Comments.Commands.AddComment
{
    public sealed class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
    {
        public AddCommentCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Escribe un comentario.")
                .MaximumLength(1000).WithMessage("El comentario no puede superar los 1000 caracteres.");
        }
    }
}
