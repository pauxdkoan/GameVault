

using GameVault.Source.Application.Dtos.Auth;

namespace GameVault.Source.Application.Interfaces.Auth
{
    public interface ILoginUseCase
    {
        Task<LoginResponse> ExecuteAsync(LoginRequest request);
    }
}
