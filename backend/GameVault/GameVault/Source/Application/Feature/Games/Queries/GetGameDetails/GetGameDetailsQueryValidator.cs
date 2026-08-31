using FluentValidation;


namespace GameVault.Source.Application.Feature.Games.Queries.GetGameDetails
{
    public class GetGameDetailsQueryValidator : AbstractValidator<GetGameDetailsQuery>
    {
        public GetGameDetailsQueryValidator() 
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Debe indicar el indificador unico.");

        }
    }
}
