using GameVault.Source.Application.Feature.Comments.Commands.AddComment;
using GameVault.Source.Application.Feature.Comments.Queries.GetReviewComments;
using GameVault.Source.Application.Feature.Reviews.Commands.CreateReview;
using GameVault.Source.Application.Feature.Reviews.Commands.DeleteReview;
using GameVault.Source.Application.Feature.Reviews.Commands.LikeReview;
using GameVault.Source.Application.Feature.Reviews.Commands.UnlikeReview;
using GameVault.Source.Application.Feature.Reviews.Commands.UpdateReview;
using GameVault.Source.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    /// <summary>
    /// Una reseña concreta: editar, borrar, me gusta y comentarios.
    /// Para listar o crear reseñas de un juego: /api/games/{externalId}/reviews.
    /// </summary>
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ISender _sender;

        public ReviewsController(ISender sender)
        {
            _sender = sender;
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReviewRequest body, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new UpdateReviewCommand(
                User.GetUserId(), id, body.Title ?? string.Empty, body.Content, body.Rating, body.ContainsSpoilers),
                cancellationToken);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteReviewCommand(User.GetUserId(), id), cancellationToken);
            return NoContent();
        }

        /// <summary>POST /api/reviews/{id}/like. Devuelve { likeCount }.</summary>
        [Authorize]
        [HttpPost("{id:guid}/like")]
        public async Task<IActionResult> Like(Guid id, CancellationToken cancellationToken)
        {
            var likeCount = await _sender.Send(new LikeReviewCommand(User.GetUserId(), id), cancellationToken);
            return Ok(new { likeCount });
        }

        /// <summary>DELETE /api/reviews/{id}/like. Devuelve { likeCount }.</summary>
        [Authorize]
        [HttpDelete("{id:guid}/like")]
        public async Task<IActionResult> Unlike(Guid id, CancellationToken cancellationToken)
        {
            var likeCount = await _sender.Send(new UnlikeReviewCommand(User.GetUserId(), id), cancellationToken);
            return Ok(new { likeCount });
        }

        [HttpGet("{id:guid}/comments")]
        public async Task<IActionResult> GetComments(
            Guid id,
            CancellationToken cancellationToken,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var response = await _sender.Send(
                new GetReviewCommentsQuery(id, User.TryGetUserId(), page, pageSize), cancellationToken);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("{id:guid}/comments")]
        public async Task<IActionResult> AddComment(Guid id, [FromBody] AddCommentRequest body, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new AddCommentCommand(User.GetUserId(), id, body.Content), cancellationToken);
            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}
