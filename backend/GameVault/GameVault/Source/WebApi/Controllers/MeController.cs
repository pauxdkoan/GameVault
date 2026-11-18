using GameVault.Source.Application.Feature.Library.Commands.RemoveLibraryEntry;
using GameVault.Source.Application.Feature.Library.Commands.UpsertLibraryEntry;
using GameVault.Source.Application.Feature.Library.Queries.GetMyLibrary;
using GameVault.Source.Application.Feature.Lists.Queries.GetMyLists;
using GameVault.Source.Application.Feature.Profile.Commands.UpdateMyProfile;
using GameVault.Source.Domain.Enums;
using GameVault.Source.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    /// <summary>
    /// Todo lo del usuario conectado: biblioteca, sus listas y su perfil.
    /// El usuario sale siempre del token, nunca de la URL: nadie puede tocar la biblioteca de otro.
    /// </summary>
    [Route("api/me")]
    [ApiController]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly ISender _sender;

        public MeController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>GET /api/me/library?status=Playing&amp;page=1&amp;pageSize=24</summary>
        [HttpGet("library")]
        public async Task<IActionResult> GetLibrary(
            CancellationToken cancellationToken,
            [FromQuery] GameStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 24)
        {
            var response = await _sender.Send(
                new GetMyLibraryQuery(User.GetUserId(), status, page, pageSize), cancellationToken);
            return Ok(response);
        }

        /// <summary>PUT /api/me/library/1942: añade el juego o actualiza su estado.</summary>
        [HttpPut("library/{externalId:int}")]
        public async Task<IActionResult> UpsertLibraryEntry(
            int externalId,
            [FromBody] UpsertLibraryEntryRequest body,
            CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new UpsertLibraryEntryCommand(
                User.GetUserId(),
                externalId,
                body.Status,
                body.ProgressPercentage,
                body.HoursPlayed,
                body.PersonalRating,
                body.StartedAt,
                body.CompletedAt,
                body.PrivateNotes), cancellationToken);

            return Ok(response);
        }

        [HttpDelete("library/{externalId:int}")]
        public async Task<IActionResult> RemoveLibraryEntry(int externalId, CancellationToken cancellationToken)
        {
            await _sender.Send(new RemoveLibraryEntryCommand(User.GetUserId(), externalId), cancellationToken);
            return NoContent();
        }

        /// <summary>GET /api/me/lists: mis listas, públicas y privadas.</summary>
        [HttpGet("lists")]
        public async Task<IActionResult> GetMyLists(CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new GetMyListsQuery(User.GetUserId()), cancellationToken);
            return Ok(response);
        }

        /// <summary>PUT /api/me/profile: devuelve el UserDto actualizado (el mismo de /api/auth/me).</summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateMyProfileRequest body, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new UpdateMyProfileCommand(
                User.GetUserId(),
                body.DisplayName,
                body.Bio,
                body.AvatarUrl,
                body.BannerUrl,
                body.Country), cancellationToken);

            return Ok(response);
        }
    }
}
