using GameVault.Source.Domain.Entities;

namespace GameVault.Source.Application.Interfaces.Security
{
    public interface ITokenProvider
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();

        /// <summary>
        /// Devuelve el hash SHA-256 del refresh token. En la base de datos solo se guarda
        /// el hash: si alguien lee la tabla, no puede usar los tokens.
        /// </summary>
        string HashToken(string token);
    }
}
