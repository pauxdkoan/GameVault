using FluentValidation;

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

            RuleForEach(x => x.GenreIds)
                .GreaterThan(0).WithMessage("El genero debe ser valido.")
                .When(x => x.GenreIds is not null);

            RuleForEach(x => x.PlatformIds)
                .GreaterThan(0).WithMessage("La plataforma debe ser valida.")
                .When(x => x.PlatformIds is not null);

            RuleFor(x => x.Sort)
                .IsInEnum().WithMessage("El orden indicado no es valido.");

        }
    }
}
