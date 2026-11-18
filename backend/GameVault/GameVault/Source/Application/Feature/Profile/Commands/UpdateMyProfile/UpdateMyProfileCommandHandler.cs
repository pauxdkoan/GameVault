using AutoMapper;
using GameVault.Source.Application.Dtos.User;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Profile.Commands.UpdateMyProfile
{
    internal sealed class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, UserDto>
    {
        private readonly GameVaultContext _context;
        private readonly IMapper _mapper;

        public UpdateMyProfileCommandHandler(GameVaultContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
                ?? throw ApiErrors.NotFound("El usuario no existe.");

            user.DisplayName = request.DisplayName.Trim();
            user.Bio = Clean(request.Bio);
            user.AvatarUrl = Clean(request.AvatarUrl);
            user.BannerUrl = Clean(request.BannerUrl);
            user.Country = Clean(request.Country);

            await _context.SaveChangesAsync(cancellationToken);

            // Devuelve el mismo UserDto que /api/auth/me para que el frontend actualice la sesión.
            return _mapper.Map<UserDto>(user);
        }

        private static string? Clean(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
