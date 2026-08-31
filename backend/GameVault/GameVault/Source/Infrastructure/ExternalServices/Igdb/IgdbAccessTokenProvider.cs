using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Interfaces.Igdb;
using GameVault.Source.Domain.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace GameVault.Source.Infrastructure.ExternalServices.Igdb
{
    public sealed class IgdbAccessTokenProvider : IIgdbAccessTokenProvider
    {
        private const string CacheKey = "IgdbAccessToken";

        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IgdbSettings _settings;

        public IgdbAccessTokenProvider(
            HttpClient httpClient,
            IMemoryCache memoryCache,
            IOptions<IgdbSettings> settings)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _settings = settings.Value;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        
        {
            if (_memoryCache.TryGetValue(CacheKey, out string? cachedToken) &&
                !string.IsNullOrWhiteSpace(cachedToken))
            {
                return cachedToken;
            }

            ValidateSettings();

            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret,
                ["grant_type"] = "client_credentials"
            });

            using var response = await PostTokenAsync(content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                    "No fue posible autenticar con IGDB.",
                    StatusCodes.Status502BadGateway);
            }

            var tokenResponse = await response.Content
                .ReadFromJsonAsync<IgdbTokenResponse>(cancellationToken: cancellationToken);

            if (tokenResponse is null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                throw new ApiException(
                    "IGDB no devolvió un token válido.",
                    StatusCodes.Status502BadGateway);
            }

            var cacheDuration = TimeSpan.FromSeconds(Math.Max(tokenResponse.ExpiresIn - 300, 60));
            _memoryCache.Set(CacheKey, tokenResponse.AccessToken, cacheDuration);

            return tokenResponse.AccessToken;
        }

        private void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(_settings.TokenUrl))
            {
                throw new InvalidOperationException("IgdbSettings:TokenUrl no está configurado.");
            }

            if (string.IsNullOrWhiteSpace(_settings.ClientId))
            {
                throw new InvalidOperationException("IgdbSettings:ClientId no está configurado.");
            }

            if (string.IsNullOrWhiteSpace(_settings.ClientSecret))
            {
                throw new InvalidOperationException("IgdbSettings:ClientSecret no está configurado.");
            }
        }

        private async Task<HttpResponseMessage> PostTokenAsync(
            HttpContent content,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _httpClient.PostAsync(_settings.TokenUrl, content, cancellationToken);
            }
            catch (OperationCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                throw new ApiException(
                    "IGDB tardó demasiado en autenticar.",
                    StatusCodes.Status504GatewayTimeout,
                    innerException: exception);
            }
            catch (HttpRequestException exception)
            {
                throw new ApiException(
                    "No fue posible conectar con IGDB para autenticar.",
                    StatusCodes.Status502BadGateway,
                    innerException: exception);
            }
        }

        private sealed class IgdbTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
        }
    }
}
