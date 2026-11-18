

using GameVault.Source.Application.Dtos.Auth;
using GameVault.Source.Application.Dtos.User;

namespace GameVault.Source.Application.Interfaces.Auth
{
    public interface IRegisterUseCase
    {
        Task<UserDto> RegisterUser(RegisterRequestDto request);
    }
}
