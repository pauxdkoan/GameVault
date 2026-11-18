using GameVault.Source.Application.Dtos.Common;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Helpers
{
    /// <summary>
    /// Paginación común para las consultas a la base de datos.
    /// Igual que en GetGamesQueryHandler, los valores fuera de rango se corrigen
    /// en lugar de devolver error (página mínima 1, tamaño entre 1 y 50).
    /// </summary>
    internal static class Paging
    {
        public const int MaxPageSize = 50;

        public static (int Page, int PageSize) Normalize(int page, int pageSize)
        {
            return (Math.Max(page, 1), Math.Clamp(pageSize, 1, MaxPageSize));
        }

        /// <summary>Cuenta el total, aplica Skip/Take y mapea cada elemento.</summary>
        public static async Task<PagedResultDto<TResult>> ToPagedResultAsync<TSource, TResult>(
            this IQueryable<TSource> query,
            int page,
            int pageSize,
            Func<TSource, TResult> map,
            CancellationToken cancellationToken)
        {
            var (safePage, safePageSize) = Normalize(page, pageSize);

            var count = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((safePage - 1) * safePageSize)
                .Take(safePageSize)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<TResult>
            {
                Items = items.Select(map).ToList(),
                Count = count,
                Page = safePage,
                PageSize = safePageSize
            };
        }
    }
}
