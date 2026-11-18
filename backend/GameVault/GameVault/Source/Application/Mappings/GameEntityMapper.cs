using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Dtos.User;
using GameVault.Source.Domain.Entities;

namespace GameVault.Source.Application.Mappings
{
    /// <summary>
    /// Convierte las entidades guardadas en la base de datos en DTOs.
    /// Así la biblioteca, las listas y las reseñas devuelven la misma
    /// tarjeta (GameCardDto) que el catálogo, sin llamar a IGDB.
    /// </summary>
    internal static class GameEntityMapper
    {
        /// <summary>
        /// Requiere haber cargado GameGenres.Genre y GamePlatforms.Platform
        /// (ver Helpers/GameQueryExtensions.cs).
        /// </summary>
        public static GameCardDto ToCard(Game game)
        {
            return new GameCardDto
            {
                ExternalId = game.ExternalId,
                Slug = game.Slug,
                Name = game.Name,
                Summary = game.Description,
                Released = game.ReleaseDate,
                CoverImage = game.CoverUrl,
                Screenshots = game.BackgroundImageUrl is null ? [] : [game.BackgroundImageUrl],
                Rating = game.ExternalRating,
                Metacritic = game.MetacriticScore,
                Genres = game.GameGenres.Select(gg => gg.Genre.Name).ToList(),
                Platforms = game.GamePlatforms
                    .Select(gp => PlatformFamilyResolver.Resolve(gp.Platform.Name))
                    .Where(family => family != Enums.PlatformFamily.Other)
                    .Distinct()
                    .ToList(),
            };
        }

        public static UserSummaryDto ToUserSummary(ApplicationUser user)
        {
            return new UserSummaryDto
            {
                UserName = user.UserName ?? string.Empty,
                DisplayName = string.IsNullOrWhiteSpace(user.DisplayName)
                    ? user.UserName ?? string.Empty
                    : user.DisplayName,
                AvatarUrl = user.AvatarUrl
            };
        }
    }
}
