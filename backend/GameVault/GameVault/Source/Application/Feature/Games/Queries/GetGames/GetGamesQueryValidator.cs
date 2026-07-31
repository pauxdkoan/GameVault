using FluentValidation;
using GameVault.Source.Application.Feature.Auth.Queries.Login;

namespace GameVault.Source.Application.Feature.Games.Queries.GetGames
{
    public class GetGamesQueryValidator: AbstractValidator<GetGamesQuery>
    {
        public GetGamesQueryValidator() 
        {
            RuleFor(x => x.Page)
                .NotEmpty().WithMessage("Debe indicar las paginas.");
            
            RuleFor(x => x.PageSize)
                .NotEmpty().WithMessage("Debe indicar el tamaño las paginas.");

        }
    }
}
