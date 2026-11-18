using FluentValidation;


namespace GameVault.Source.Application.Feature.Games.Queries.SearchGame
{
    public class SearchGamesValidator : AbstractValidator<SearchGamesQuery>
    {
        public SearchGamesValidator() 
        {

            RuleFor(x=>x.Query)
                .NotEmpty().WithMessage("Debes proporcionar un término de búsqueda.");

            RuleFor(x => x.Page)
                .NotEmpty().WithMessage("Debe indicar las paginas.");
            
            RuleFor(x => x.PageSize)
                .NotEmpty().WithMessage("Debe indicar el tamaño las paginas.");

        }
    }
}
