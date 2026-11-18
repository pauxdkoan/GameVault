using GameVault.Source.Application.Dtos.User;
using MediatR;

namespace GameVault.Source.Application.Feature.Auth.Queries.GetCurrentUser
{
    /// <summary>Datos del usuario dueño del token (GET /api/auth/me).</summary>
    public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto>;
}
