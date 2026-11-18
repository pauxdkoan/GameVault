using GameVault.Source.Application.Enums;

namespace GameVault.Source.Application.Dtos.Game
{
    /// <summary>Opciones que el catálogo muestra en sus filtros.</summary>
    public sealed class GameFiltersDto
    {
        public List<FilterOptionDto> Platforms { get; set; } = [];

        public List<FilterOptionDto> Genres { get; set; } = [];

        public List<SortOptionDto> Sorts { get; set; } = [];
    }

    /// <summary>Una opción de filtro: el id de IGDB y el texto a mostrar.</summary>
    public sealed record FilterOptionDto(int Id, string Label);

    /// <summary>Una opción de orden: el valor del enum y el texto a mostrar.</summary>
    public sealed record SortOptionDto(GameSort Value, string Label);
}
