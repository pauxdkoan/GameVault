using GameVault.Source.Application.Feature.Activity.Queries.GetActivity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    [Route("api/activity")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ISender _sender;

        public ActivityController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>GET /api/activity?page=1: últimas reseñas, juegos completados y listas públicas.</summary>
        [HttpGet]
        public async Task<IActionResult> GetActivity(
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _sender.Send(new GetActivityQuery(page, pageSize), cancellationToken);
            return Ok(response);
        }
    }
}
