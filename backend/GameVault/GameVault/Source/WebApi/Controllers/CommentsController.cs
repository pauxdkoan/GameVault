using GameVault.Source.Application.Feature.Comments.Commands.DeleteComment;
using GameVault.Source.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    /// <summary>Un comentario concreto. Para listar o crear: /api/reviews/{id}/comments.</summary>
    [Route("api/comments")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ISender _sender;

        public CommentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteCommentCommand(User.GetUserId(), id), cancellationToken);
            return NoContent();
        }
    }
}
