using FluentValidation;
using GameVault.Source.Application.Feature.Lists.Commands.CreateList;

namespace GameVault.Source.Application.Feature.Lists.Commands.UpdateList
{
    public sealed class UpdateListCommandValidator : AbstractValidator<UpdateListCommand>
    {
        public UpdateListCommandValidator()
        {
            Include(new ListContentRules<UpdateListCommand>(x => x.Name, x => x.Description));
        }
    }
}
