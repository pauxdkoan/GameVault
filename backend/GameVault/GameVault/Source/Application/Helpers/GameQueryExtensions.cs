using GameVault.Source.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Helpers
{
    /// <summary>Includes que necesita GameEntityMapper.ToCard, para no repetirlos en cada consulta.</summary>
    internal static class GameQueryExtensions
    {
        public static IQueryable<UserGame> IncludeCardData(this IQueryable<UserGame> query) =>
            query
                .Include(ug => ug.Game).ThenInclude(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                .Include(ug => ug.Game).ThenInclude(g => g.GamePlatforms).ThenInclude(gp => gp.Platform);

        public static IQueryable<GameListItem> IncludeCardData(this IQueryable<GameListItem> query) =>
            query
                .Include(i => i.Game).ThenInclude(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                .Include(i => i.Game).ThenInclude(g => g.GamePlatforms).ThenInclude(gp => gp.Platform);
    }
}
