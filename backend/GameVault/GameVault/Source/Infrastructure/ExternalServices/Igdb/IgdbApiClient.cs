using GameVault.Source.Application.Dtos.Igdb;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Interfaces.Igdb;
using GameVault.Source.Domain.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace GameVault.Source.Infrastructure.ExternalServices.Igdb
{
    public sealed class IgdbApiClient : IIgdbApiClient
    {

        //Campos de igdb para filtrar la data
        private const string GameFields =
             "fields id,slug,name,summary,first_release_date," +
            "cover.image_id,screenshots.image_id," +
            "rating,rating_count," +
            "aggregated_rating,aggregated_rating_count," +
            "total_rating,total_rating_count," +
            "hypes," +
            "genres.name,genres.slug," +
            "platforms.name,platforms.slug;";

        private readonly HttpClient _httpClient;
        private readonly IIgdbAccessTokenProvider _accessTokenProvider;
        private readonly IgdbSettings _settings;

        public IgdbApiClient(
            HttpClient httpClient,
            IIgdbAccessTokenProvider accessTokenProvider,
            IOptions<IgdbSettings> settings)
        {
            _httpClient = httpClient;
            _accessTokenProvider = accessTokenProvider;
            _settings = settings.Value;
        }

        public async Task<IgdbGamesResponse> GetGamesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
        {
            var offset = (page - 1) * pageSize;

            const string filters =
                "where version_parent = null " +
                "& cover != null " +
                "& total_rating != null " +
                "& total_rating_count >= 50 " +
                "& themes != (42);";

            var query =
                $"{GameFields} " +
                $"{filters} " +
                $"sort total_rating_count desc; " +
                $"limit {pageSize}; " +
                $"offset {offset};";

            return await GetGamesPageAsync(
                query,
                filters,
                cancellationToken);
        }

        public async Task<IgdbGamesResponse> SearchGamesAsync(
            string search,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var offset = (page - 1) * pageSize;
            var safeSearch = EscapeIgdbString(search);
            const string filters = "where version_parent = null & cover != null & themes != (42);";
            
            var query = $"search \"{safeSearch}\"; {GameFields} {filters} limit {pageSize}; offset {offset};";
            var countQuery = $"search \"{safeSearch}\"; {filters}";

            return await GetGamesPageAsync(query, countQuery, cancellationToken);
        }

        public async Task<IgdbGamesResponse> GetTrendingGamesAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var offset = (page - 1) * pageSize;

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

            var query =
                $"{GameFields} " +
                $"{filters} " +
                $"sort total_rating desc; " +
                $"limit {pageSize}; " +
                $"offset {offset};";

            return await GetGamesPageAsync(
                query,
                filters,
                cancellationToken);
        }




        public async Task<IgdbGamesResponse> GetUpcomingGamesAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var offset = (page - 1) * pageSize;

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

            var query =
                $"{GameFields} " +
                $"{filters} " +
                $"sort first_release_date asc; " +
                $"limit {pageSize}; " +
                $"offset {offset};";

            return await GetGamesPageAsync(
                query,
                filters,
                cancellationToken);
        }


        public async Task<IgdbGameDto?> GetGameByIdAsync(
            long id,
            CancellationToken cancellationToken)
        {
            var query = $"{GameFields} where id = {id}; limit 1;";

            var games = await PostAsync<List<IgdbGameDto>>(
                "games",
                query,
                cancellationToken);

            return games?.FirstOrDefault();
        }


        private async Task<IgdbGamesResponse> GetGamesPageAsync(
            string gamesQuery,
            string countQuery,
            CancellationToken cancellationToken)
        {
            var games = await PostAsync<List<IgdbGameDto>>("games", gamesQuery, cancellationToken)
                ?? [];
            var count = await CountAsync(countQuery, cancellationToken);

            return new IgdbGamesResponse
            {
                Count = count,
                Results = games
            };
        }

        private async Task<T?> PostAsync<T>(
            string endpoint,
            string body,
            CancellationToken cancellationToken)
        {
            ValidateSettings();

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = new StringContent(body, Encoding.UTF8, "text/plain");

            await AddIgdbHeadersAsync(request, cancellationToken);

            using var response = await SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                    "No fue posible obtener videojuegos desde IGDB.",
                    StatusCodes.Status502BadGateway);
            }

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }


        private async Task<int> CountAsync(string body, CancellationToken cancellationToken)
        {
            ValidateSettings();

            using var request = new HttpRequestMessage(HttpMethod.Post, "games/count");
            request.Content = new StringContent(body, Encoding.UTF8, "text/plain");

            await AddIgdbHeadersAsync(request, cancellationToken);

            using var response = await SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                    "No fue posible contar videojuegos desde IGDB.",
                    StatusCodes.Status502BadGateway);
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return ReadCount(json);
        }
        
        private async Task AddIgdbHeadersAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var accessToken = await _accessTokenProvider.GetAccessTokenAsync(cancellationToken);

            request.Headers.TryAddWithoutValidation("Client-ID", _settings.ClientId);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _httpClient.SendAsync(request, cancellationToken);
            }
            catch (OperationCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                throw new ApiException(
                    "IGDB tardó demasiado en responder.",
                    StatusCodes.Status504GatewayTimeout,
                    innerException: exception);
            }
            catch (HttpRequestException exception)
            {
                throw new ApiException(
                    "No fue posible conectar con IGDB.",
                    StatusCodes.Status502BadGateway,
                    innerException: exception);
            }
        }

        private void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
            {
                throw new InvalidOperationException("IgdbSettings:BaseUrl no está configurado.");
            }

            if (string.IsNullOrWhiteSpace(_settings.ClientId))
            {
                throw new InvalidOperationException("IgdbSettings:ClientId no está configurado.");
            }
        }

        private static int ReadCount(string json)
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty("count", out var objectCount))
            {
                return ReadCountValue(objectCount);
            }

            if (root.ValueKind == JsonValueKind.Array &&
                root.GetArrayLength() > 0 &&
                root[0].TryGetProperty("count", out var arrayCount))
            {
                return ReadCountValue(arrayCount);
            }

            return 0;
        }

        private static int ReadCountValue(JsonElement element)
        {
            if (element.TryGetInt32(out var count))
            {
                return count;
            }

            if (element.TryGetInt64(out var longCount))
            {
                return longCount > int.MaxValue ? int.MaxValue : (int)longCount;
            }

            return 0;
        }

        private static string EscapeIgdbString(string value)
        {
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }
    }
}
