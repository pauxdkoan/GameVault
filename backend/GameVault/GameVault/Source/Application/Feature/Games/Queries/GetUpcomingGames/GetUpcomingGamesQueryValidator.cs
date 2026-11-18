using FluentValidation;

namespace GameVault.Source.Application.Feature.Games.Queries.GetUpcomingGames
{
    public class GetUpcomingGamesQueryValidator : AbstractValidator<GetUpcomingGamesQuery>
    {
        public GetUpcomingGamesQueryValidator() 
        {
            RuleFor(x => x.Page)
                .NotEmpty().WithMessage("Debe indicar las paginas.");
            
            RuleFor(x => x.PageSize)
                .NotEmpty().WithMessage("Debe indicar el tamaño las paginas.");

        }
    }
}
