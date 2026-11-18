using GameVault.Source.Application.Dtos.User;
using MediatR;

namespace GameVault.Source.Application.Feature.Profile.Commands.UpdateMyProfile
{
    /// <summary>Edita los datos públicos del perfil. El nombre de usuario y el correo no cambian aquí.</summary>
    public sealed record UpdateMyProfileCommand(
        Guid UserId,
        string DisplayName,
        string? Bio,
        string? AvatarUrl,
        string? BannerUrl,
        string? Country) : IRequest<UserDto>;

    /// <summary>Cuerpo del PUT /api/me/profile.</summary>
    public sealed record UpdateMyProfileRequest(
        string DisplayName,
        string? Bio,
        string? AvatarUrl,
        string? BannerUrl,
        string? Country);
}
