using AutoMapper;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Interfaces.Security;
using GameVault.Source.Domain.Contants;
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
        IRequestHandler<LoginWithRefreshTokenQuery, Response>
    {


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenProvider _jwtTokenGenerator;
        private readonly JwtSettings _jwtSettings;
        private readonly GameVaultContext _context;
        private readonly IMapper _mapper;
        public LoginWithRefreshTokenQueryHandler(UserManager<ApplicationUser> userManager, ITokenProvider jwtTokenGenerator, 
            IOptions<JwtSettings> jwtOptions,
            GameVaultContext gameVaultContext,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwtSettings = jwtOptions.Value;
            _context = gameVaultContext;
            _mapper = mapper;
        }

        public async Task<Response> Handle(LoginWithRefreshTokenQuery request, CancellationToken cancellationToken)
        {

            RefreshToken refreshToken = await _context.RefreshTokens
               .Include(r => r.User)
               .FirstOrDefaultAsync(r=> r.TokenHash == request.RefreshToken);

            if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new ApiException("The refresh token has expired", StatusCodes.Status401Unauthorized);

            }

            var roles = await _userManager.GetRolesAsync(refreshToken.User);
            var accesToken = _jwtTokenGenerator.GenerateToken(refreshToken.User, roles);

            refreshToken.TokenHash = _jwtTokenGenerator.GenerateRefreshToken();

            refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.ExpirationMinutes);

            await _context.SaveChangesAsync(cancellationToken);

            return new Response(accesToken, refreshToken.TokenHash);

        }
    }

  


  }


