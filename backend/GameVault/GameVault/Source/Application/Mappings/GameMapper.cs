using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Dtos.Igdb;

namespace GameVault.Source.Application.Mappings
{
    internal static class GameMapper
    {
        public static GameSummaryDto GameSummary(IgdbGameDto game)
        {
            return new GameSummaryDto
            {
                ExternalId = game.Id,
                Slug = game.Slug,
                Name = game.Name,
                Summary= game.Summary,
                Released = ToDateOnly(game.FirstReleaseDate),
                Screenshot = GetScreenshotUrl(game).FirstOrDefault(),
                CoverImage = GetImageUrl(game.Cover, "cover_big_2x"),
                Rating = ToFivePointRating(game.TotalRating ?? game.Rating),
                Metacritic = ToWholeRating(game.AggregatedRating),
                Genres = game.Genres
                    .Select(genre => genre.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList(),
                Platforms = game.Platforms
                    .Select(platform => platform.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList()
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
                Screenshots = GetScreenshotUrl(game),
                CoverImage = GetImageUrl(game.Cover, "cover_big_2x"),
                Rating = ToFivePointRating(game.TotalRating ?? game.Rating),
                Metacritic = ToWholeRating(game.AggregatedRating),
                Genres = game.Genres
                    .Select(genre => genre.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList(),
                Platforms = game.Platforms
                    .Select(platform => platform.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList()
            };
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

        private static List<string>? GetScreenshotUrl(IgdbGameDto game)
        {
            var screenshots=  new List<string>();
            var gameScreenshots = game.Screenshots.ToList();

            foreach (var screenshot in gameScreenshots) 
            {
                screenshots.Add(GetImageUrl(screenshot, "screenshot_huge"));
            }

            return screenshots;
      
        }

        private static string? GetImageUrl(IgdbImageDto? image, string size)
        {
            if (image is null || string.IsNullOrWhiteSpace(image.ImageId))
            {
                return null;
            }

            return $"https://images.igdb.com/igdb/image/upload/t_{size}/{image.ImageId}.jpg";
        }

        private static decimal ToFivePointRating(double? rating)
        {
            if (rating is null)
            {
                return 0;
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
