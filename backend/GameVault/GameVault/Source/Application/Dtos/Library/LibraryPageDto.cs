using GameVault.Source.Application.Dtos.Common;
using GameVault.Source.Domain.Enums;

namespace GameVault.Source.Application.Dtos.Library
{
    /// <summary>
    /// Una página de la biblioteca más cuántos juegos hay en cada estado,
    /// para pintar las pestañas ("Jugando 3", "Completados 12"...) sin otra petición.
    /// </summary>
    public sealed class LibraryPageDto
    {
        public PagedResultDto<LibraryEntryDto> Entries { get; set; } = new();

        public Dictionary<GameStatus, int> StatusCounts { get; set; } = [];
    }
}
