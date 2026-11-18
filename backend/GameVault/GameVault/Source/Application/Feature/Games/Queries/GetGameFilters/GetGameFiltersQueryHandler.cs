using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Enums;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.GetGameFilters
{
    /// <summary>
    /// Devuelve las opciones de filtro del catálogo.
    /// Antes los ids de IGDB estaban escritos a mano en el componente de Angular;
    /// ahora viven aquí y el frontend solo los pinta. Los ids son los oficiales
    /// de IGDB (https://api-docs.igdb.com) y los textos están en español.
    /// </summary>
    public sealed class GetGameFiltersQueryHandler
        : IRequestHandler<GetGameFiltersQuery, GameFiltersDto>
    {
        private static readonly List<FilterOptionDto> Platforms =
        [
            new(6, "PC"),
            new(167, "PlayStation 5"),
            new(48, "PlayStation 4"),
            new(169, "Xbox Series X|S"),
            new(49, "Xbox One"),
            new(130, "Nintendo Switch"),
            new(34, "Android"),
            new(39, "iOS"),
        ];

        private static readonly List<FilterOptionDto> Genres =
        [
            new(31, "Aventura"),
            new(12, "RPG"),
            new(5, "Shooter"),
            new(32, "Indie"),
            new(15, "Estrategia"),
            new(8, "Plataformas"),
            new(9, "Puzles"),
            new(10, "Carreras"),
            new(14, "Deportes"),
            new(4, "Lucha"),
            new(13, "Simulación"),
            new(25, "Hack and slash"),
            new(16, "Por turnos"),
            new(24, "Táctico"),
            new(33, "Arcade"),
            new(34, "Novela visual"),
        ];

        private static readonly List<SortOptionDto> Sorts =
        [
            new(GameSort.Popular, "Popularidad"),
            new(GameSort.Rating, "Valoración"),
            new(GameSort.ReleaseDate, "Lanzamiento"),
        ];

        public Task<GameFiltersDto> Handle(
            GetGameFiltersQuery request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new GameFiltersDto
            {
                Platforms = Platforms,
                Genres = Genres,
                Sorts = Sorts,
            });
        }
    }
}
