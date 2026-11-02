
using GameVault.Source.Application.Enums;
using GameVault.Source.Application.Feature.Games.Queries.GetGameDetails;
using GameVault.Source.Application.Feature.Games.Queries.GetGames;
using GameVault.Source.Application.Feature.Games.Queries.GetTrendingGames;
using GameVault.Source.Application.Feature.Games.Queries.GetUpcomingGames;
using GameVault.Source.Application.Feature.Games.Queries.SearchGame;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    [Route("api/games")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ISender _sender;

        public GamesController(ISender sender)
        {
            _sender = sender;
        }


        [HttpGet("get-games")]
        public async Task<IActionResult> GetGames(
            CancellationToken cancellationToken,
            [FromQuery] int page=1,
            [FromQuery] int pageSize=20,
            [FromQuery] string? search = null,
            [FromQuery] int[]? genreIds = null,
            [FromQuery] int[]? platformIds = null,
            [FromQuery] GameSort sort = GameSort.Popular

            )
        {
            var response = await _sender.Send(new GetGamesQuery(
                Page:page, 
                PageSize:pageSize, 
                Search: search,
                GenreIds: genreIds,
                PlatformIds: platformIds,
                Sort: sort ), cancellationToken);

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchGames(
           CancellationToken cancellationToken,
           [FromQuery] string query,
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 20

           )
        {
            var response = await _sender.Send(new SearchGamesQuery(query,page, pageSize), cancellationToken);

            return Ok(response);
        }

        [HttpGet("get-trending-games")]
        public async Task<IActionResult> GetTrengindGames(
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var response = await _sender.Send(new GetTrendingGamesQuery(page, pageSize), cancellationToken);
           return Ok(response);

        }

        [HttpGet("get-upcomming-games")]
        public async Task<IActionResult> GetUpCommingGames(
           CancellationToken cancellationToken,
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 20)
        {
            var response = await _sender.Send(new GetUpcomingGamesQuery(page, pageSize), cancellationToken);
            return Ok(response);

        }

        [HttpGet("get-game-details")]
        public async Task<IActionResult> GetGameDetails(
            CancellationToken cancellationToken,
            [FromQuery] long id
            )
        {
            var response = await _sender.Send(new GetGameDetailsQuery(id), cancellationToken);
            if(response == null)
            {
                return NoContent();
            }
            return Ok(response);

        }

        //private static int[]? NormalizeFilterIds(int[]? ids, int? singleId)
        //{
        //    int[] selectedIds = ids is { Length: > 0 }
        //        ? ids
        //        : singleId.HasValue
        //            ? new[] { singleId.Value }
        //            : Array.Empty<int>();

        //    var normalizedIds = selectedIds
        //        .Distinct()
        //        .ToArray();

        //    return normalizedIds.Length > 0 ? normalizedIds : null;
        //}




    }
}
