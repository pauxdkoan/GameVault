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

namespace GameVault.Source.Application.Feature.Auth.Commands.RevokeRefreshToken
{

    public sealed record RevokeRefreshTokenCommand(Guid userId):IRequest<bool>;
    internal sealed class RevokeRefreshTokenCommandHandler :
        IRequestHandler<RevokeRefreshTokenCommand, bool>
    {

        private readonly GameVaultContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RevokeRefreshTokenCommandHandler(GameVaultContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = _userManager.FindByIdAsync(request.userId.ToString());

            if (user == null) 
            {
                throw new ApiException("No puedes hacer esta accón", StatusCodes.Status401Unauthorized);
            }
            await _context.RefreshTokens
                .Where(r => r.UserId == request.userId)
                .ExecuteDeleteAsync();
            
            return true;
          
        }
    }


}
