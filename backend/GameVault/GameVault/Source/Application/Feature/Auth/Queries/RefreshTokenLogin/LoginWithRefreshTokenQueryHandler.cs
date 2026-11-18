using AutoMapper;
using GameVault.Source.Application.Dtos.User;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Interfaces.Security;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Domain.Settings;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameVault.Source.Application.Feature.Auth.Queries.Login
{
    internal sealed class LoginWithRefreshTokenQueryHandler :
        IRequestHandler<LoginWithRefreshTokenQuery, LoginResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly JwtSettings _jwtSettings;
        private readonly GameVaultContext _context;
        private readonly IMapper _mapper;

        public LoginWithRefreshTokenQueryHandler(
            UserManager<ApplicationUser> userManager,
            ITokenProvider tokenProvider,
            IOptions<JwtSettings> jwtOptions,
            GameVaultContext gameVaultContext,
            IMapper mapper)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            _jwtSettings = jwtOptions.Value;
            _context = gameVaultContext;
            _mapper = mapper;
        }

        public async Task<LoginResponse> Handle(
            LoginWithRefreshTokenQuery request,
            CancellationToken cancellationToken)
        {
            var tokenHash = _tokenProvider.HashToken(request.RefreshToken);

            RefreshToken? currentToken = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);

            // Caducado, revocado o de un usuario desactivado: hay que volver a iniciar sesión.
            if (currentToken is null || !currentToken.IsActive || !currentToken.User.IsActive)
            {
                throw new ApiException(
                    "La sesión ha caducado. Inicia sesión de nuevo.",
                    StatusCodes.Status401Unauthorized);
            }

            // Rotación: el token usado queda revocado y se emite uno nuevo.
            var plainRefreshToken = _tokenProvider.GenerateRefreshToken();
            var newToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                TokenHash = _tokenProvider.HashToken(plainRefreshToken),
                UserId = currentToken.UserId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            };

            currentToken.RevokedAt = DateTime.UtcNow;
            currentToken.ReplacedByTokenHash = newToken.TokenHash;
            _context.RefreshTokens.Add(newToken);

            await _context.SaveChangesAsync(cancellationToken);

            var roles = await _userManager.GetRolesAsync(currentToken.User);
            var accessToken = _tokenProvider.GenerateToken(currentToken.User, roles);

            return new LoginResponse(
                accessToken,
                plainRefreshToken,
                DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                _mapper.Map<UserDto>(currentToken.User));
        }
    }
}
