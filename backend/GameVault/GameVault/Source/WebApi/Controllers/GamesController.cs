using GameVault.Source.Application.Enums;
using GameVault.Source.Application.Feature.Games.Queries.GetGameDetails;
using GameVault.Source.Application.Feature.Games.Queries.GetGameFilters;
using GameVault.Source.Application.Feature.Games.Queries.GetGames;
using GameVault.Source.Application.Feature.Games.Queries.GetTrendingGames;
using GameVault.Source.Application.Feature.Games.Queries.GetUpcomingGames;
using GameVault.Source.Application.Feature.Games.Queries.SearchGame;
using GameVault.Source.Application.Feature.Reviews.Commands.CreateReview;
using GameVault.Source.Application.Feature.Reviews.Queries.GetGameReviews;
using GameVault.Source.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    /// <summary>
    /// Juegos (datos de IGDB). Rutas REST: el recurso es "games" y el verbo
    /// lo pone HTTP, así que no hace falta "get-" en la URL.
    /// </summary>
    [Route("api/games")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ISender _sender;

        public GamesController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>GET /api/games?page=1&amp;pageSize=20&amp;search=&amp;genreIds=&amp;platformIds=&amp;sort=Popular</summary>
        [HttpGet]
        public async Task<IActionResult> GetGames(
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] int[]? genreIds = null,
            [FromQuery] int[]? platformIds = null,
            [FromQuery] GameSort sort = GameSort.Popular)
        {
            var response = await _sender.Send(new GetGamesQuery(
                Page: page,
                PageSize: pageSize,
                Search: search,
                GenreIds: genreIds,
                PlatformIds: platformIds,
                Sort: sort), cancellationToken);

            return Ok(response);
        }

        /// <summary>GET /api/games/search?query=zelda</summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchGames(
            CancellationToken cancellationToken,
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var response = await _sender.Send(new SearchGamesQuery(query, page, pageSize), cancellationToken);
            return Ok(response);
        }

        /// <summary>GET /api/games/trending</summary>
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingGames(
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var response = await _sender.Send(new GetTrendingGamesQuery(page, pageSize), cancellationToken);
            return Ok(response);
        }

        /// <summary>GET /api/games/upcoming</summary>
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingGames(
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var response = await _sender.Send(new GetUpcomingGamesQuery(page, pageSize), cancellationToken);
            return Ok(response);
        }

        /// <summary>GET /api/games/filters: opciones de plataforma, género y orden.</summary>
        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters(CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new GetGameFiltersQuery(), cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// GET /api/games/1942. Si el juego no existe, el handler lanza
        /// ApiException 404 y el GlobalExceptionHandler responde con ProblemDetails.
        /// Con sesión, incluye el juego en la biblioteca del usuario (MyLibraryEntry).
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetGameDetails(
            long id,
            CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new GetGameDetailsQuery(id, User.TryGetUserId()), cancellationToken);
            return Ok(response);
        }

        /// <summary>GET /api/games/1942/reviews: reseñas, media de la comunidad y la reseña propia.</summary>
        [HttpGet("{externalId:int}/reviews")]
        public async Task<IActionResult> GetReviews(
            int externalId,
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _sender.Send(
                new GetGameReviewsQuery(externalId, User.TryGetUserId(), page, pageSize),
                cancellationToken);
            return Ok(response);
        }

        /// <summary>POST /api/games/1942/reviews: escribe una reseña (una por usuario y juego).</summary>
        [Authorize]
        [HttpPost("{externalId:int}/reviews")]
        public async Task<IActionResult> CreateReview(
            int externalId,
            [FromBody] ReviewRequest body,
            CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new CreateReviewCommand(
                User.GetUserId(),
                externalId,
                body.Title ?? string.Empty,
                body.Content,
                body.Rating,
                body.ContainsSpoilers), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}
