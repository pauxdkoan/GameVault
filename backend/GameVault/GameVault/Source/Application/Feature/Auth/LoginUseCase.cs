
using GameVault.Source.Application.Dtos.Auth;
using GameVault.Source.Application.Interfaces.Auth;
using GameVault.Source.Application.Interfaces.Security;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Domain.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace GameVault.Source.Application.Feature.Auth
{
    public class LoginUseCase : ILoginUseCase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly JwtSettings _jwtSettings;

        public LoginUseCase(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator, IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<LoginResponse> ExecuteAsync(LoginRequest request)
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
            if(user is null)
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

            var roles= await _userManager.GetRolesAsync(user);
            var token= _jwtTokenGenerator.GenerateToken(user, roles);

            return new LoginResponse 
            { 
                Token = token,
                Email = request.Email,
                ExpiresAt= DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                UserId= user.Id.ToString(),
                Roles = roles
            };
        }
    }
}
