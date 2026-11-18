using GameVault.Source.Application.Feature.Lists.Commands.AddGameToList;
using GameVault.Source.Application.Feature.Lists.Commands.CreateList;
using GameVault.Source.Application.Feature.Lists.Commands.DeleteList;
using GameVault.Source.Application.Feature.Lists.Commands.RemoveGameFromList;
using GameVault.Source.Application.Feature.Lists.Commands.UpdateList;
using GameVault.Source.Application.Feature.Lists.Queries.GetListDetail;
using GameVault.Source.Application.Feature.Lists.Queries.GetPublicLists;
using GameVault.Source.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    /// <summary>Listas de juegos. Mis listas (incluidas las privadas): GET /api/me/lists.</summary>
    [Route("api/lists")]
    [ApiController]
    public class ListsController : ControllerBase
    {
        private readonly ISender _sender;

        public ListsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>GET /api/lists: listas públicas de la comunidad.</summary>
        [HttpGet]
        public async Task<IActionResult> GetPublicLists(
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {
            var response = await _sender.Send(new GetPublicListsQuery(page, pageSize), cancellationToken);
            return Ok(response);
        }

        /// <summary>GET /api/lists/{id}: una lista con sus juegos (las privadas solo para su dueño).</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetail(Guid id, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new GetListDetailQuery(id, User.TryGetUserId()), cancellationToken);
            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ListRequest body, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(
                new CreateListCommand(User.GetUserId(), body.Name, body.Description, body.IsPublic), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ListRequest body, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(
                new UpdateListCommand(User.GetUserId(), id, body.Name, body.Description, body.IsPublic), cancellationToken);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteListCommand(User.GetUserId(), id), cancellationToken);
            return NoContent();
        }

        /// <summary>POST /api/lists/{id}/games/1942 con body opcional { note }.</summary>
        [Authorize]
        [HttpPost("{id:guid}/games/{externalId:int}")]
        public async Task<IActionResult> AddGame(
            Guid id,
            int externalId,
            [FromBody] AddGameToListRequest? body,
            CancellationToken cancellationToken)
        {
            var response = await _sender.Send(
                new AddGameToListCommand(User.GetUserId(), id, externalId, body?.Note), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [Authorize]
        [HttpDelete("{id:guid}/games/{externalId:int}")]
        public async Task<IActionResult> RemoveGame(Guid id, int externalId, CancellationToken cancellationToken)
        {
            await _sender.Send(new RemoveGameFromListCommand(User.GetUserId(), id, externalId), cancellationToken);
            return NoContent();
        }
    }
}
