using FluentValidation;
using GameVault.Source.Application.Feature.Reviews.Commands.CreateReview;

namespace GameVault.Source.Application.Feature.Reviews.Commands.UpdateReview
{
    public sealed class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewCommandValidator()
        {
            Include(new ReviewContentRules<UpdateReviewCommand>(
                x => x.Title, x => x.Content, x => x.Rating));
        }
    }
}
