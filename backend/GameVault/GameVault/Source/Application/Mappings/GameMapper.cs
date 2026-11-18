using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Dtos.Game.AdditionalDtos;
using GameVault.Source.Application.Dtos.Igdb;
using GameVault.Source.Application.Dtos.Igdb.AdttionalDtos;

namespace GameVault.Source.Application.Mappings
{
    /// <summary>
    /// Convierte las respuestas de IGDB en los DTOs que devuelve la API.
    /// </summary>
    internal static class GameMapper
    {
        private const string CoverSize = "cover_big_2x";
        private const string ScreenshotSize = "screenshot_huge";

        /// <summary>Tarjeta para listados (catálogo, tendencias, próximos).</summary>
        public static GameCardDto GameCard(IgdbGameDto game)
        {
            var videos = GetVideoUrls(game);

            return new GameCardDto
            {
                ExternalId = game.Id,
                Slug = game.Slug,
                Name = game.Name,
                Summary = game.Summary,
                Released = ToDateOnly(game.FirstReleaseDate),
                CoverImage = GetImageUrl(game.Cover, CoverSize),
                Screenshots = GetScreenshotUrls(game).Take(3).ToList(),
                TrailerVideo = GetTrailerVideoUrl(videos),
                Rating = ToFivePointRating(game.TotalRating ?? game.Rating),
                Metacritic = ToWholeRating(game.AggregatedRating),
                Genres = GetGenreNames(game.Genres),
                Platforms = PlatformFamilyResolver.ToFamilies(game.Platforms),
            };
        }

        /// <summary>
        /// Tarjeta para juegos relacionados (similares, DLCs, expansiones).
        /// IGDB devuelve menos datos de ellos, así que no hay resumen ni capturas.
        /// </summary>
        public static GameCardDto RelatedGameCard(IgdbRelatedGameDto game)
        {
            return new GameCardDto
            {
                ExternalId = game.Id,
                Slug = game.Slug,
                Name = game.Name,
                Released = ToDateOnly(game.FirstReleaseDate),
                CoverImage = GetImageUrl(game.Cover, CoverSize),
                Rating = ToFivePointRating(game.TotalRating),
                Genres = GetGenreNames(game.Genres),
                Platforms = PlatformFamilyResolver.ToFamilies(game.Platforms),
            };
        }

        public static GameDetailsDto GameDetails(IgdbGameDto game)
        {
            return new GameDetailsDto
            {
                ExternalId = game.Id,
                Summary = game.Summary,
                Slug = game.Slug,
                Name = game.Name,
                Released = ToDateOnly(game.FirstReleaseDate),

                CoverImage = GetImageUrl(game.Cover, CoverSize),
                Rating = ToFivePointRating(game.TotalRating ?? game.Rating),
                Metacritic = ToWholeRating(game.AggregatedRating),

                Genres = GetGenreNames(game.Genres),
                Platforms = PlatformFamilyResolver.ToPlatforms(game.Platforms),

                Videos = GetVideoUrls(game),
                Screenshots = GetScreenshotUrls(game),

                Dlcs = ToRelatedCards(game.Dlcs),
                Expansions = ToRelatedCards(game.Expansions),
                ExpandedGames = ToRelatedCards(game.ExpandedGames),
                SimilarGames = ToRelatedCards(game.SimilarGames),

                GameModes = game.GameModes.Select(x => x.Name).ToList(),
                Companies = game.InvolvedCompanies.Select(x => new InvolvedCompaniesDto
                {
                    Name = x.Name,
                    IsDeveloper = x.Developer,
                    IsPublisher = x.Publisher,
                    LogoImage = x.CompanyDto?.Logo?.ImageId,
                }).ToList(),
                StoryLine = game.StoryLine,
                WebSites = game.Websites.Select(x => new WebSiteDto
                {
                    Name = x.Url,
                    Url = x.Url,
                }).ToList(),
                GameType = game.GameType?.Type,
                Themes = game.Themes.Select(x => x.Name).ToList(),
                ParentGame = game.ParentGame?.Name

                //TODO: MAPEAR LAS TIENDAS...
            };
        }

        private static List<GameCardDto> ToRelatedCards(List<IgdbRelatedGameDto>? games)
        {
            if (games is null)
            {
                return [];
            }

            return games
                .Where(game => game.Id > 0 && !string.IsNullOrWhiteSpace(game.Name))
                .Select(RelatedGameCard)
                .ToList();
        }

        private static DateOnly? ToDateOnly(long? unixSeconds)
        {
            if (unixSeconds is null)
            {
                return null;
            }

            return DateOnly.FromDateTime(
                DateTimeOffset.FromUnixTimeSeconds(unixSeconds.Value).UtcDateTime);
        }

        private static List<string> GetScreenshotUrls(IgdbGameDto game)
        {
            if (game.Screenshots is null)
            {
                return [];
            }

            return game.Screenshots
                .Select(screenshot => GetImageUrl(screenshot, ScreenshotSize))
                .OfType<string>()
                .ToList();
        }

        private static List<GameVideoDto> GetVideoUrls(IgdbGameDto game)
        {
            if (game.Videos is null)
            {
                return [];
            }

            return game.Videos
                .Where(video => !string.IsNullOrWhiteSpace(video.VideoId))
                .Select(video => new GameVideoDto
                {
                    Name = video.Name ?? string.Empty,
                    Url = $"https://www.youtube.com/watch?v={video.VideoId}",
                    EmbedUrl = $"https://www.youtube.com/embed/{video.VideoId}",
                    ThumbnailUrl = $"https://img.youtube.com/vi/{video.VideoId}/hqdefault.jpg",
                })
                .ToList();
        }

        private static string? GetTrailerVideoUrl(IEnumerable<GameVideoDto> videos)
        {
            var trailer = videos.FirstOrDefault(video =>
                string.Equals(video.Name, "Trailer", StringComparison.OrdinalIgnoreCase))
                ?? videos.FirstOrDefault(video =>
                    video.Name.Contains("Trailer", StringComparison.OrdinalIgnoreCase));

            return trailer?.Url;
        }

        private static List<string> GetGenreNames(List<IgdbGenreDto>? genres)
        {
            if (genres is null)
            {
                return [];
            }

            return genres
                .Select(genre => genre.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();
        }

        private static string? GetImageUrl(IgdbCoverDto? image, string size)
        {
            if (image is null || string.IsNullOrWhiteSpace(image.ImageId))
            {
                return null;
            }

            return $"https://images.igdb.com/igdb/image/upload/t_{size}/{image.ImageId}.jpg";
        }

        private static decimal? ToFivePointRating(double? rating)
        {
            if (rating is null)
            {
                return null;
            }

            return Math.Round((decimal)rating.Value / 20, 1);
        }

        private static int? ToWholeRating(double? rating)
        {
            if (rating is null)
            {
                return null;
            }

            return (int)Math.Round(rating.Value);
        }
    }
}
