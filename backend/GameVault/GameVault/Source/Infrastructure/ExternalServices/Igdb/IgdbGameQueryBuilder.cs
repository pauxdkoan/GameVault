using GameVault.Source.Application.Enums;

namespace GameVault.Source.Infrastructure.ExternalServices.Igdb
{
    internal static class IgdbGameQueryBuilder
    {
        private const string GameFields =
            "fields id,slug,name,summary,first_release_date," +
            "videos.name,videos.video_id," +
            "cover.image_id,screenshots.image_id," +
            "rating,rating_count," +
            "aggregated_rating,aggregated_rating_count," +
            "total_rating,total_rating_count," +
            "hypes," +
            "genres.name,genres.slug," +
            "themes.name,keywords.name," +
            "platforms.name,platforms.platform_family.name,platforms.slug";

        private const string GameDetailFields =
            "storyline," +
            "artworks.image_id," +
            "game_modes.name," +
            "game_type.type," +
            "external_games.name," +
            "external_games.external_game_source.name," +
            "external_games.uid," +
            "external_games.url," +

            "involved_companies.company.name," +
            "involved_companies.company.logo.image_id," +
            "involved_companies.developer," +
            "involved_companies.publisher," +
            "age_ratings.organization.name," +
            "age_ratings.rating_category.rating," +
            "age_ratings.rating_cover_url," +

            "dlcs.name," +
            "dlcs.cover.image_id," +
            "dlcs.genres.name," +
            "dlcs.total_rating," +
            "dlcs.platforms.name," +

            "similar_games.name," +
            "similar_games.cover.image_id," +
            "similar_games.genres.name," +
            "similar_games.total_rating," +
            "similar_games.platforms.name," +

            "expansions.name," +
            "expansions.cover.image_id," +
            "expansions.genres.name," +
            "expansions.total_rating," +
            "expansions.platforms.name," +

            "websites.type.type," +
            "websites.url," +


            "expanded_games.cover.image_id," +
            "expanded_games.genres.name," +
            "expanded_games.total_rating," +
            "expanded_games.platforms.name," +
            "expanded_games.name";

        public static IgdbPageQuery BuildGamesPage(
            string? search,
            IReadOnlyCollection<int>? genreIds,
            IReadOnlyCollection<int>? platformIds,
            GameSort sort,
            int page,
            int pageSize)
        {
            var offset = GetOffset(page, pageSize);
            var searchClause = BuildSearchClause(search);
            var filters = BuildGamesFilters(search, genreIds, platformIds, sort);
            var sortClause = BuildSortClause(sort);

            var resultsQuery =
                $"{searchClause}" +
                $"{GameFields}; " +
                $"{filters} " +
                $"{sortClause} " +
                $"limit {pageSize}; " +
                $"offset {offset};";

            var countQuery = $"{searchClause}{filters}";

            return new IgdbPageQuery(resultsQuery, countQuery);
        }

     

        public static IgdbPageQuery BuildSearchGamesPage(
            string search,
            int page,
            int pageSize)
        {
            var offset = GetOffset(page, pageSize);
            var searchClause = BuildSearchClause(search);
            const string filters = "where version_parent = null & cover != null & themes != (42);";

            var resultsQuery =
                $"{searchClause}" +
                $"{GameFields}; " +
                $"{filters} " +
                $"limit {pageSize}; " +
                $"offset {offset};";

            var countQuery = $"{searchClause}{filters}";

            return new IgdbPageQuery(resultsQuery, countQuery);
        }

        public static IgdbPageQuery BuildTrendingGamesPage(int page, int pageSize)
        {
            var offset = GetOffset(page, pageSize);
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var sixMonthsAgo = DateTimeOffset.UtcNow
                .AddMonths(-6)
                .ToUnixTimeSeconds();

            var filters =
                $"where version_parent = null " +
                $"& cover != null " +
                $"& total_rating != null " +
                $"& first_release_date >= {sixMonthsAgo} " +
                $"& first_release_date <= {now} " +
                $"& themes != (42);";

            var resultsQuery =
                $"{GameFields}; " +
                $"{filters} " +
                $"sort total_rating desc; " +
                $"limit {pageSize}; " +
                $"offset {offset};";

            return new IgdbPageQuery(resultsQuery, filters);
        }

        public static IgdbPageQuery BuildUpcomingGamesPage(int page, int pageSize)
        {
            var offset = GetOffset(page, pageSize);
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var sixMonthsFromNow = DateTimeOffset.UtcNow
                .AddMonths(6)
                .ToUnixTimeSeconds();

            var filters =
                $"where version_parent = null " +
                $"& cover != null " +
                $"& first_release_date >= {now} " +
                $"& first_release_date <= {sixMonthsFromNow} " +
                $"& themes != (42);";

            var resultsQuery =
                $"{GameFields}; " +
                $"{filters} " +
                $"sort first_release_date asc; " +
                $"limit {pageSize}; " +
                $"offset {offset};";

            return new IgdbPageQuery(resultsQuery, filters);
        }

        public static string BuildGameDetailPageById(long id)
        {
            return $"{GameFields},{GameDetailFields}; where id = {id}; limit 1;";
        }

        private static string BuildGamesFilters(
            string? search,
            IReadOnlyCollection<int>? genreIds,
            IReadOnlyCollection<int>? platformIds,
            GameSort sort)
        {
            var filters = new List<string>
            {
                "version_parent = null",
                "cover != null",
                "themes != (42)"
            };

            var hasSearch = !string.IsNullOrWhiteSpace(search);
            var shouldUseDefaultQualityFilters =
                !hasSearch &&
                IsNullOrEmpty(genreIds) &&
                IsNullOrEmpty(platformIds) &&
                sort == GameSort.Popular;

            if (shouldUseDefaultQualityFilters)
            {
                filters.Add("total_rating != null");
                filters.Add("total_rating_count >= 50");
            }
            else if (!hasSearch && sort == GameSort.Popular)
            {
                filters.Add("total_rating != null");
            }
            else if (sort == GameSort.Rating)
            {
                filters.Add("total_rating != null");
            }
            else if (sort == GameSort.ReleaseDate)
            {
                filters.Add("first_release_date != null");
            }

            var genreFilter = BuildIdsFilter("genres", genreIds);
            if (genreFilter is not null)
            {
                filters.Add(genreFilter);
            }

            var platformFilter = BuildIdsFilter("platforms", platformIds);
            if (platformFilter is not null)
            {
                filters.Add(platformFilter);
            }

            return $"where {string.Join(" & ", filters)};";
        }

        private static string BuildSearchClause(string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return string.Empty;
            }

            return $"search \"{EscapeIgdbString(search.Trim())}\"; ";
        }

        private static string BuildSortClause(GameSort sort)
        {
            return sort switch
            {
                GameSort.Rating => "sort total_rating desc;",
                GameSort.ReleaseDate => "sort first_release_date desc;",
                _ => "sort total_rating_count desc;"
            };
        }

        private static string? BuildIdsFilter(
            string field,
            IReadOnlyCollection<int>? ids)
        {
            if (ids is null || ids.Count == 0)
            {
                return null;
            }

            var validIds = ids
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            return validIds.Count switch
            {
                0 => null,
                1 => $"{field} = {validIds[0]}",
                _ => $"{field} = ({string.Join(",", validIds)})"
            };
        }

        private static bool IsNullOrEmpty(IReadOnlyCollection<int>? ids)
        {
            return ids is null || ids.Count == 0;
        }

        private static int GetOffset(int page, int pageSize)
        {
            return (page - 1) * pageSize;
        }

        private static string EscapeIgdbString(string value)
        {
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }
    }
}
