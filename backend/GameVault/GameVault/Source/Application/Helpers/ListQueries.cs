using GameVault.Source.Application.Dtos.Lists;
using GameVault.Source.Application.Mappings;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Helpers
{
    /// <summary>
    /// Consulta común para el resumen de una lista: dueño, total de juegos
    /// y las 4 primeras portadas. Se hace en dos consultas (listas y portadas)
    /// para no cargar todos los juegos de cada lista.
    /// </summary>
    internal static class ListQueries
    {
        private const int CoversPerList = 4;

        internal sealed class ListRow
        {
            public GameList List { get; init; } = null!;
            public ApplicationUser Owner { get; init; } = null!;
            public int GameCount { get; init; }
        }

        public static IQueryable<ListRow> ToRows(this IQueryable<GameList> query)
        {
            return query.Select(l => new ListRow
            {
                List = l,
                Owner = l.ApplicationUser,
                GameCount = l.Items.Count
            });
        }

        /// <summary>Carga las portadas de todas las listas de una vez y arma los resúmenes.</summary>
        public static async Task<List<GameListSummaryDto>> ToSummariesAsync(
            this GameVaultContext context,
            IReadOnlyCollection<ListRow> rows,
            CancellationToken cancellationToken)
        {
            var listIds = rows.Select(row => row.List.Id).ToList();

            var covers = await context.GameListItems
                .AsNoTracking()
                .Where(i => listIds.Contains(i.GameListId) && i.Game.CoverUrl != null)
                .OrderBy(i => i.Position)
                .Select(i => new { i.GameListId, Cover = i.Game.CoverUrl! })
                .ToListAsync(cancellationToken);

            var coversByList = covers
                .GroupBy(c => c.GameListId)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Cover).Take(CoversPerList).ToList());

            return rows.Select(row => new GameListSummaryDto
            {
                Id = row.List.Id,
                Name = row.List.Name,
                Description = row.List.Description,
                IsPublic = row.List.IsPublic,
                GameCount = row.GameCount,
                CoverImages = coversByList.GetValueOrDefault(row.List.Id) ?? [],
                Owner = GameEntityMapper.ToUserSummary(row.Owner),
                CreatedAt = row.List.CreatedAt,
                UpdatedAt = row.List.UpdatedAt
            }).ToList();
        }
    }
}
