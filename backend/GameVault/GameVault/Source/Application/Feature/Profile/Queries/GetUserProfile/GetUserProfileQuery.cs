using GameVault.Source.Application.Dtos.Profile;
using MediatR;

namespace GameVault.Source.Application.Feature.Profile.Queries.GetUserProfile
{
    public sealed record GetUserProfileQuery(string UserName, Guid? CurrentUserId) : IRequest<ProfileDto>;
}
