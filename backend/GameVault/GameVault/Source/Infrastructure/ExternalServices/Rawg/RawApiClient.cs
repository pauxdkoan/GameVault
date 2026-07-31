using GameVault.Source.Application.Dtos.Rawg.Game;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Interfaces.Rawg;
using GameVault.Source.Domain.Settings;
using Microsoft.Extensions.Options;
using System.Runtime;

namespace GameVault.Source.Infrastructure.ExternalServices.Rawg
{
    public sealed class RawApiClient : IRawgApiClient
    {

        private readonly HttpClient _httpClient;
        private readonly RawgSettings _rawgSettings;

        public RawApiClient(
            HttpClient httpClient, 
            IOptions<RawgSettings> rawgSettings)
        {
            _httpClient = httpClient;
            _rawgSettings = rawgSettings.Value;
        }

        public async Task<RawgGamesResponse> GetGamesAsync(
            int page, 
            int pageSize, 
            CancellationToken cancellationToken)
        {
            var url = $"games?key={Uri.EscapeDataString(_rawgSettings.ApiKey)}" +
            $"&page={page}&page_size={pageSize}";

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                "No fue posible obtener los videojuegos.",
                StatusCodes.Status502BadGateway);
            }

            var result = await response.Content
                .ReadFromJsonAsync<RawgGamesResponse>(cancellationToken:cancellationToken);

            return result ?? new RawgGamesResponse();
        }

        public async Task<RawgGamesResponse> SearchGamesAsync(string search, int page, int pageSize, CancellationToken cancellationToken)
        {
           var url = $"games?key={Uri.EscapeDataString(_rawgSettings.ApiKey)}" +
            $"&search={Uri.EscapeDataString(search)}" +
            $"&page={page}&page_size={pageSize}";

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                    "No fue posible buscar los videojuegos.",
                    StatusCodes.Status502BadGateway);
            }

            var result= await response.Content
                .ReadFromJsonAsync<RawgGamesResponse>(cancellationToken: cancellationToken);

            return result ?? new RawgGamesResponse();



        }
    }
}
