using GameVault.Source.Application.Feature.Profile.Queries.GetUserProfile;
using GameVault.Source.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>GET /api/users/paulo: perfil público con estadísticas, reseñas recientes y listas.</summary>
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetProfile(string userName, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(new GetUserProfileQuery(userName, User.TryGetUserId()), cancellationToken);
            return Ok(response);
        }
    }
}
