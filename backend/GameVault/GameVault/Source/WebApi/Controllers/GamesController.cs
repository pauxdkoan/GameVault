
using GameVault.Source.Application.Feature.Games.Queries.GetGames;
using GameVault.Source.Application.Feature.Games.Queries.SearchGame;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    [Route("api/games")]
    [ApiController]
    public class GamesController : Controller
    {
        private readonly ISender _sender;

        public GamesController(ISender sender)
        {
            _sender = sender;
        }


        [HttpGet()]
        public async Task<IActionResult> GetGames(
            CancellationToken cancellationToken,
            [FromQuery] int page=1,
            [FromQuery] int pageSize=20
            
            )
        {
            var response = await _sender.Send(new GetGamesQuery(page, pageSize), cancellationToken);

            return Ok(response);
        }

        [HttpGet("{query}")]
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
    }
}
