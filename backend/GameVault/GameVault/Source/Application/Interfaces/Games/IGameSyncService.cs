using GameVault.Source.Domain.Entities;

namespace GameVault.Source.Application.Interfaces.Games
{
    public interface IGameSyncService
    {
        /// <summary>
        /// Devuelve el juego guardado en la base de datos con ese id de IGDB.
        /// Si todavía no existe, lo trae de IGDB y lo guarda (con géneros y plataformas).
        /// Lo usan la biblioteca, las reseñas y las listas, que apuntan a Game.Id.
        /// </summary>
        Task<Game> EnsureGameAsync(int externalId, CancellationToken cancellationToken);
    }
}
