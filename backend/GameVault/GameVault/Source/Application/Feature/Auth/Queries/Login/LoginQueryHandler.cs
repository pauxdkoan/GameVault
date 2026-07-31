using AutoMapper;
using GameVault.Source.Application.Dtos.Auth;
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
using System.Net;

namespace GameVault.Source.Application.Feature.Auth.Queries.Login
{
    internal sealed class LoginQueryHandler :
        IRequestHandler<LoginQuery, LoginResponse>
    {


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenProvider _jwtTokenGenerator;
        private readonly JwtSettings _jwtSettings;
        private readonly GameVaultContext _context;
        private readonly IMapper _mapper;
        public LoginQueryHandler(UserManager<ApplicationUser> userManager, ITokenProvider jwtTokenGenerator, 
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

        public async Task<LoginResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.Select(u => new
            {
                u.Id,
                u.Email,
                u.NormalizedEmail,
                u.UserName,
                u.NormalizedUserName
            }).ToListAsync();


            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("El usuario está inactivo");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                TokenHash = _jwtTokenGenerator.GenerateRefreshToken(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
               
                
            };

             _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new LoginResponse
            (
              token,
              refreshToken.TokenHash,
              DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
              _mapper.Map<UserDto>(user)


            );


        }
    }

  


  }


