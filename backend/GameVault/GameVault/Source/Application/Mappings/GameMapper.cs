using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Dtos.Game.AdditionalDtos;
using GameVault.Source.Application.Dtos.Igdb;
using GameVault.Source.Application.Dtos.Igdb.AdttionalDtos;
using GameVault.Source.Application.Interfaces.Igdb;
using System;
using System.Linq;
using System.Xml.Linq;

namespace GameVault.Source.Application.Mappings
{
    internal static class GameMapper
    {
      
       

        public static GameSummaryDto GameSummary(IgdbGameDto game)
        {
            var screenshots = GetScreenshotUrls(game);
            var videos = GetVideoUrls(game);

            return new GameSummaryDto
            {
                ShortSummary= GetShortSummary(game),
                ExternalId = game.Id,
                Slug = game.Slug,
                Name = game.Name,
                Summary = game.Summary,
                Released = ToDateOnly(game.FirstReleaseDate),
                Screenshots = GetScreenshotUrls(game).Take(3).ToList(),
                CoverImage = GetImageUrl(game.Cover, "cover_big_2x"),
                Rating = ToFivePointRating(game.TotalRating ?? game.Rating),
                Metacritic = ToWholeRating(game.AggregatedRating),
                TrailerVideo = GetTrailerVideoUrl(videos),
                Genres = GetGenreNames(game.Genres),

                PlatformsBadges = game.Platforms
                .Select(x=> GetPlatformSummary(x.Name))
                .Where(x=>!string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList(),

                Themes = game.Themes.Select(x => x.Name).ToList(),


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

                CoverImage = GetImageUrl(game.Cover, "cover_big_2x"),
                Rating = ToFivePointRating(game.TotalRating ?? game.Rating),
                Metacritic = ToWholeRating(game.AggregatedRating),

                Genres = GetGenreNames(game.Genres),
                Platforms = GetPlatformNames(game.Platforms),

                Videos = GetVideoUrls(game),
                Screenshots = GetScreenshotUrls(game),
                Dlcs = game.Dlcs.Select(x => new RelatedGameDto
                {
                    ExternalId = x.Id,
                    CoverImage = GetImageUrl(x.Cover, "cover_big_2x"),
                    Genres = GetGenreNames(x.Genres),
                    Name = x.Name,
                    Platforms = GetPlatformNames(x.Platforms),
                    Raiting = ToFivePointRating(x.TotalRating)

                }).ToList(),
                Expansions = game.Expansions.Select(x => new RelatedGameDto
                {
                    ExternalId = x.Id,
                    CoverImage = GetImageUrl(x.Cover, "cover_big_2x"),
                    Genres = GetGenreNames(x.Genres),
                    Name = x.Name,
                    Platforms = GetPlatformNames(x.Platforms),
                    Raiting = ToFivePointRating(x.TotalRating)

                }).ToList(),
                ExpandedGames = game.ExpandedGames.Select(x => new RelatedGameDto
                {
                    ExternalId = x.Id,
                    CoverImage = GetImageUrl(x.Cover, "cover_big_2x"),
                    Genres = GetGenreNames(x.Genres),
                    Name = x.Name,
                    Platforms = GetPlatformNames(x.Platforms),
                    Raiting = ToFivePointRating(x.TotalRating)

                }).ToList(),
                GameModes = game.GameModes.Select(x => x.Name).ToList(),
                Companies = game.InvolvedCompanies.Select(x => new InvolvedCompaniesDto
                {
                    Name = x.Name,
                    IsDeveloper = x.Developer,
                    IsPublisher = x.Publisher,
                    LogoImage = x.CompanyDto?.Logo?.ImageId,

                }).ToList(),
                SimilarGames = game.SimilarGames.Select(x => new RelatedGameDto
                {
                    ExternalId = x.Id,
                    CoverImage = GetImageUrl(x.Cover, "cover_big_2x"),
                    Genres = GetGenreNames(x.Genres),
                    Name = x.Name,
                    Platforms = GetPlatformNames(x.Platforms),
                    Raiting = ToFivePointRating(x.TotalRating)

                }).ToList(),
                StoryLine = game.StoryLine,
                WebSites = game.Websites.Select(x => new WebSiteDto
                {
                    Name = x.Url,
                    Url = x.Url,
                }).ToList(),
                GameType = game?.GameType?.Type,
                Themes = game.Themes.Select(x => x.Name).ToList(),
                ParentGame = game?.ParentGame?.Name


                //TODO: MAPEAR LAS TIENDAS...

            };
        }


      
        private static string GetShortSummary(IgdbGameDto game) 
        {
            if (game.Summary == null) 
            { return null; 
            
            }

            var index = game.Summary.IndexOf('.');
            
            if(index == -1)
            {
                return game.Summary;
            }
            return game.Summary.Substring(0, index);

            
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
            var screenshots = new List<string>();

            if (game.Screenshots is null)
            {
                return screenshots;
            }

            foreach (var screenshot in game.Screenshots)
            {
                var imageUrl = GetImageUrl(screenshot, "screenshot_huge");

                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    screenshots.Add(imageUrl);
                }
            }

            return screenshots;
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

        private static List<string> GetGenreNames(List<IgdbGenreDto> genres)
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


        private static string? GetPlatformSummary(string name)
        {
            if (name.Contains("Windows", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("PC", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Linux", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Mac", StringComparison.OrdinalIgnoreCase))
                return "PC";

            if (name.Contains("PlayStation", StringComparison.OrdinalIgnoreCase))
                return "PlayStation";

            if (name.Contains("Xbox", StringComparison.OrdinalIgnoreCase))
                return "Xbox";

            if (name.Contains("Nintendo", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Switch", StringComparison.OrdinalIgnoreCase))
                return "Nintendo";

            if (name.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("iOS", StringComparison.OrdinalIgnoreCase))
                return "Mobile";

            if (name.Contains("VR", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Oculus", StringComparison.OrdinalIgnoreCase)||
                name.Contains("Quest", StringComparison.OrdinalIgnoreCase))

                return "VR";



            return null;



        }

        private static List<string> GetPlatformNames(List<IgdbPlatformDto> platforms)
        {
            if (platforms is null)
            {
                return [];
            }

            return platforms
                .Select(platform => platform.Name)
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
