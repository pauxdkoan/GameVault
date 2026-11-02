using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Interfaces.Igdb;
using GameVault.Source.Domain.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace GameVault.Source.Infrastructure.ExternalServices.Igdb
{
    public sealed class IgdbRequestClient
    {
        private readonly HttpClient _httpClient;
        private readonly IIgdbAccessTokenProvider _accessTokenProvider;
        private readonly IgdbSettings _settings;

        public IgdbRequestClient(
            HttpClient httpClient,
            IIgdbAccessTokenProvider accessTokenProvider,
            IOptions<IgdbSettings> settings)
        {
            _httpClient = httpClient;
            _accessTokenProvider = accessTokenProvider;
            _settings = settings.Value;
        }

        public async Task<T?> PostAsync<T>(
            string endpoint,
            string body,
            CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = new StringContent(body, Encoding.UTF8, "text/plain");

            await AddIgdbHeadersAsync(request, cancellationToken);

            using var response = await SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                    "No fue posible obtener datos desde IGDB.",
                    StatusCodes.Status502BadGateway);
            }

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }

        public async Task<int> CountAsync(
            string endpoint,
            string body,
            CancellationToken cancellationToken)
        {
            var response = await PostAsync<IgdbCountResponse>(
                endpoint,
                body,
                cancellationToken);

            return response?.Count ?? 0;
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

        private sealed class IgdbCountResponse
        {
            [JsonPropertyName("count")]
            public int Count { get; set; }
        }
    }
}
