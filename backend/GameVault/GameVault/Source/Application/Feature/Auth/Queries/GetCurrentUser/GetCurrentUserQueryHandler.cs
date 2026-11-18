using AutoMapper;
using GameVault.Source.Application.Dtos.User;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GameVault.Source.Application.Feature.Auth.Queries.GetCurrentUser
{
    internal sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user is null || !user.IsActive)
            {
                throw new ApiException("La sesión no es válida.", StatusCodes.Status401Unauthorized);
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}
