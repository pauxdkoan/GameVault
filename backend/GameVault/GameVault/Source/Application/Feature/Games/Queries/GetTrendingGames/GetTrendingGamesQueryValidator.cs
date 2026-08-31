using FluentValidation;

namespace GameVault.Source.Application.Feature.Games.Queries.GetTrendingGames
{
    public class GetTrendingGamesQueryValidator : AbstractValidator<GetTrendingGamesQuery>
    {
        public GetTrendingGamesQueryValidator() 
        {
            RuleFor(x => x.Page)
                .NotEmpty().WithMessage("Debe indicar las paginas.");
            
            RuleFor(x => x.PageSize)
                .NotEmpty().WithMessage("Debe indicar el tamaño las paginas.");

        }
    }
}
