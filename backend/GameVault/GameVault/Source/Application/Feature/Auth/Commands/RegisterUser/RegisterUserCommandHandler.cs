using AutoMapper;
using GameVault.Source.Application.Dtos.User;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Interfaces.Security;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Domain.Settings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GameVault.Source.Application.Feature.Auth.Commands.RegisterUser
{
    internal sealed class RegisterUserCommandHandler :
        IRequestHandler<RegisterUserCommand, UserDto>
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenProvider _jwtTokenGenerator;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager, ITokenProvider jwtTokenGenerator, IOptions<JwtSettings> jwtOptions, IMapper mapper)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwtSettings = jwtOptions.Value;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {

            var userWithSameUsername = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUsername != null)
            {
                throw new ApiException(
                 "No fue posible completar el registro.",
                 StatusCodes.Status409Conflict,
                 new Dictionary<string, string[]>
                 {
                     ["userName"] =
                     [
                         "El nombre de usario ya está registrado."
                     ]
                 });
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null)
            {
                throw new ApiException(
                 "No fue posible completar el registro.",
                 StatusCodes.Status409Conflict,
                 new Dictionary<string, string[]>
                 {
                     ["email"] =
                     [
                         "El correo ya está registrado."
                     ]
                 });
            }

            var user = new ApplicationUser
            {
                DisplayName = request.Name,
                Email = request.Email,
                UserName = request.UserName,
                IsActive = true,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.ToList();
                throw new ApiException(
                "No fue posible completar el registro.",
                StatusCodes.Status500InternalServerError,
                null);
            }



            return _mapper.Map<UserDto>(user);
        }
    }


}
