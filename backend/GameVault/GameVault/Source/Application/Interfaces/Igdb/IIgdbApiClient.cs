using GameVault.Source.Application.Dtos.Igdb;
using GameVault.Source.Application.Enums;

namespace GameVault.Source.Application.Interfaces.Igdb
{
    public interface IIgdbApiClient
    {
        Task<IgdbGamesResponse> GetGamesAsync(
            string? search,
            IReadOnlyCollection<int>? genreIds,
            IReadOnlyCollection<int>? platformIds,
            GameSort sort,
            int page,
            int pageSize,
            CancellationToken cancellationToken);

        Task<IgdbGamesResponse> SearchGamesAsync(
            string search,
            int page,
            int pageSize,
            CancellationToken cancellationToken);

        Task<IgdbGamesResponse> GetTrendingGamesAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken);

        Task<IgdbGamesResponse> GetUpcomingGamesAsync(
           int page,
           int pageSize,
           CancellationToken cancellationToken);

        Task<IgdbGameDto?> GetGameByIdAsync(
            long id,
            CancellationToken cancellationToken);
        }


}
