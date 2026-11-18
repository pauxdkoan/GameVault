using GameVault.Source.Application.Dtos.Auth;
using GameVault.Source.Application.Feature.Auth.Commands.RegisterUser;
using GameVault.Source.Application.Feature.Auth.Commands.RevokeRefreshToken;
using GameVault.Source.Application.Feature.Auth.Queries.GetCurrentUser;
using GameVault.Source.Application.Feature.Auth.Queries.Login;
using GameVault.Source.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginQuery query,
            CancellationToken cancellationToken)
        {
            var response = await _sender.Send(query, cancellationToken);
            return Ok(response);
        }

        [HttpPost("refresh-tokens")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] LoginWithRefreshTokenQuery query,
            CancellationToken cancellationToken)
        {
            var response = await _sender.Send(query, cancellationToken);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterUserCommand command,
            CancellationToken cancellationToken)
        {
            var response = await _sender.Send(command, cancellationToken);
            return Ok(response);
        }

        /// <summary>Usuario de la sesión actual. Requiere el access token.</summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me(CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new GetCurrentUserQuery(User.GetUserId()), cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Cierra la sesión revocando el refresh token.
        /// No exige access token: así funciona aunque este ya haya caducado.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            [FromBody] RevokeRefreshTokenCommand command,
            CancellationToken cancellationToken)
        {
            await _sender.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
