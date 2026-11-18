using GameVault.Source.Application.Dtos.Game;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.GetGameFilters
{
    /// <summary>Pide las opciones de filtro del catálogo (plataformas, géneros, orden).</summary>
    public sealed record GetGameFiltersQuery : IRequest<GameFiltersDto>;
}
