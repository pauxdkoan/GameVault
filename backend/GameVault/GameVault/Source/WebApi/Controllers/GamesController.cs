
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
            [FromQuery] int pageSize=20
            
            )
        {
            var response = await _sender.Send(new GetGamesQuery(page, pageSize), cancellationToken);

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




    }
}
