using GameVault.Source.Application.Interfaces.Security;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Auth.Commands.RevokeRefreshToken
{
    /// <summary>
    /// Cierra la sesión: revoca el refresh token que envía el cliente.
    /// Solo quien tiene el token puede revocarlo, así nadie puede cerrar la sesión de otro.
    /// </summary>
    public sealed record RevokeRefreshTokenCommand(string RefreshToken) : IRequest<bool>;

    internal sealed class RevokeRefreshTokenCommandHandler :
        IRequestHandler<RevokeRefreshTokenCommand, bool>
    {
        private readonly GameVaultContext _context;
        private readonly ITokenProvider _tokenProvider;

        public RevokeRefreshTokenCommandHandler(GameVaultContext context, ITokenProvider tokenProvider)
        {
            _context = context;
            _tokenProvider = tokenProvider;
        }

        public async Task<bool> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var tokenHash = _tokenProvider.HashToken(request.RefreshToken);

            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);

            // Si no existe o ya estaba revocado, el resultado es el mismo: sesión cerrada.
            if (token is null || token.RevokedAt is not null)
            {
                return true;
            }

            token.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
