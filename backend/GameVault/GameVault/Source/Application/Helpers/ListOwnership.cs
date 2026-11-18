using GameVault.Source.Application.Exceptions;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Helpers
{
    internal static class ListOwnership
    {
        /// <summary>
        /// Carga una lista para modificarla. 404 si no existe, 403 si es de otro usuario.
        /// Todos los comandos de listas empiezan así.
        /// </summary>
        public static async Task<GameList> GetOwnedListAsync(
            this GameVaultContext context,
            Guid listId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var list = await context.GameLists.FirstOrDefaultAsync(l => l.Id == listId, cancellationToken)
                ?? throw ApiErrors.NotFound("La lista no existe.");

            if (list.ApplicationUserId != userId)
            {
                throw ApiErrors.Forbidden("Solo el dueño puede modificar esta lista.");
            }

            return list;
        }
    }
}
