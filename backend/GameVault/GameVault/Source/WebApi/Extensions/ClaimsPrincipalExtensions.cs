using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GameVault.Source.WebApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Id del usuario autenticado, leído del claim "sub" del JWT.
        /// ASP.NET lo renombra a NameIdentifier al validar el token, por eso se miran los dos.
        /// </summary>
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!Guid.TryParse(value, out var userId))
            {
                throw new UnauthorizedAccessException("Token sin usuario válido.");
            }

            return userId;
        }

        /// <summary>
        /// Id del usuario si la petición trae un token válido; null si es anónima.
        /// Para endpoints públicos que muestran algo extra con sesión (ej: "te gusta").
        /// </summary>
        public static Guid? TryGetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }
}
