using GameVault.Source.Domain.Entities;

namespace GameVault.Source.Application.Interfaces.Security
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
    }
}
