using GameVault.Source.Application.Dtos.Rawg.Game;

namespace GameVault.Source.Application.Interfaces.Rawg
{
    public interface IRawgApiClient
    {
        Task<RawgGamesResponse> GetGamesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);

        Task<RawgGamesResponse> SearchGamesAsync(
            string search,
            int page,
            int pageSize,
            CancellationToken cancellationToken);

    }
}
