using System.Net;
using System.Text;
using GameVault.Source.Application.Enums;
using GameVault.Source.Application.Interfaces.Igdb;
using GameVault.Source.Domain.Settings;
using GameVault.Source.Infrastructure.ExternalServices.Igdb;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace GameVault.Tests.Igdb
{
    /// <summary>
    /// Catálogo (GET /api/games) contra un IGDB falso: se comprueba qué consultas
    /// se envían y cómo se ordenan las respuestas, sin llamar a IGDB de verdad.
    /// </summary>
    public class IgdbApiClientTests
    {
        [Fact]
        public async Task La_busqueda_no_envia_sort_y_mantiene_los_filtros()
        {
            var igdb = new FakeIgdb();
            var client = CreateClient(igdb);

            await client.GetGamesAsync("zelda", [12], [48], GameSort.Rating, 1, 20, CancellationToken.None);

            var gamesQuery = igdb.Bodies("games").First();
            Assert.Contains("search \"zelda\";", gamesQuery);
            Assert.DoesNotContain("sort ", gamesQuery);
            Assert.Contains("genres = 12", gamesQuery);
            Assert.Contains("platforms = 48", gamesQuery);
        }

        [Fact]
        public async Task Popular_sin_filtros_usa_la_popularidad_actual_en_orden()
        {
            var igdb = new FakeIgdb
            {
                // Ranking de IGDB: 30 (GTA VI), 10, 20 (este no tiene portada) y 40.
                Popularity = "[{\"game_id\":30,\"value\":0.9},{\"game_id\":10,\"value\":0.5},{\"game_id\":20,\"value\":0.4},{\"game_id\":40,\"value\":0.1}]",
                ValidIds = "[{\"id\":10},{\"id\":30},{\"id\":40}]",
                // IGDB devuelve los juegos desordenados.
                PageGames = "[{\"id\":10,\"name\":\"Diez\"},{\"id\":30,\"name\":\"GTA VI\"}]",
            };
            var client = CreateClient(igdb);

            var page = await client.GetGamesAsync(null, null, null, GameSort.Popular, 1, 2, CancellationToken.None);

            Assert.Equal(3, page.Count);
            Assert.Equal(["GTA VI", "Diez"], page.Results.Select(game => game.Name));
            Assert.Contains("where popularity_type = 1;", igdb.Bodies("popularity_primitives").Single());
            Assert.Contains("where id = (30,10)", igdb.Bodies("games").Last());
        }

        [Fact]
        public async Task El_ranking_popular_se_guarda_en_cache()
        {
            var igdb = new FakeIgdb
            {
                Popularity = "[{\"game_id\":1,\"value\":1}]",
                ValidIds = "[{\"id\":1}]",
                PageGames = "[{\"id\":1,\"name\":\"Uno\"}]",
            };
            var client = CreateClient(igdb);

            await client.GetGamesAsync(null, null, null, GameSort.Popular, 1, 20, CancellationToken.None);
            await client.GetGamesAsync(null, null, null, GameSort.Popular, 1, 20, CancellationToken.None);

            Assert.Single(igdb.Bodies("popularity_primitives"));
        }

        [Fact]
        public async Task Si_la_popularidad_falla_se_usa_el_orden_de_siempre()
        {
            var igdb = new FakeIgdb { PopularityFails = true };
            var client = CreateClient(igdb);

            await client.GetGamesAsync(null, null, null, GameSort.Popular, 1, 20, CancellationToken.None);

            Assert.Contains("sort total_rating_count desc;", igdb.Bodies("games").Last());
        }

        [Fact]
        public async Task Popular_con_filtros_usa_el_orden_de_siempre()
        {
            var igdb = new FakeIgdb();
            var client = CreateClient(igdb);

            await client.GetGamesAsync(null, [12], null, GameSort.Popular, 1, 20, CancellationToken.None);

            Assert.Empty(igdb.Bodies("popularity_primitives"));
            Assert.Contains("sort total_rating_count desc;", igdb.Bodies("games").Single());
        }

        private static IgdbApiClient CreateClient(FakeIgdb igdb)
        {
            var http = new HttpClient(igdb) { BaseAddress = new Uri("https://igdb.test/v4/") };
            var settings = Options.Create(new IgdbSettings { ClientId = "test" });
            var requestClient = new IgdbRequestClient(http, new FakeToken(), settings);
            return new IgdbApiClient(requestClient, new MemoryCache(new MemoryCacheOptions()));
        }

        private sealed class FakeToken : IIgdbAccessTokenProvider
        {
            public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken) => Task.FromResult("token");
        }

        /// <summary>IGDB de mentira: guarda cada consulta y responde según el endpoint.</summary>
        private sealed class FakeIgdb : HttpMessageHandler
        {
            private readonly List<(string Endpoint, string Body)> _requests = [];

            public string Popularity { get; init; } = "[]";
            public bool PopularityFails { get; init; }
            public string ValidIds { get; init; } = "[]";
            public string PageGames { get; init; } = "[]";

            public IEnumerable<string> Bodies(string endpoint) =>
                _requests.Where(request => request.Endpoint == endpoint).Select(request => request.Body);

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var endpoint = request.RequestUri!.AbsolutePath.Replace("/v4/", "");
                var body = await request.Content!.ReadAsStringAsync(cancellationToken);
                _requests.Add((endpoint, body));

                if (endpoint == "popularity_primitives" && PopularityFails)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var json = endpoint switch
                {
                    "popularity_primitives" => Popularity,
                    "games/count" => "{\"count\":0}",
                    "games" when body.StartsWith("fields id;") => ValidIds,
                    "games" when body.Contains("where id = (") => PageGames,
                    _ => "[]"
                };

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            }
        }
    }
}
