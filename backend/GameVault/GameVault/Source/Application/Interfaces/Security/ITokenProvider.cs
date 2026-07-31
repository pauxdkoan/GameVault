using GameVault.Source.Domain.Entities;

namespace GameVault.Source.Application.Interfaces.Security
{
    public interface ITokenProvider
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
    }
}
