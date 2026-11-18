using MediatR;

namespace GameVault.Source.Application.Feature.Auth.Queries.Login
{
    /// <summary>
    /// Renueva la sesión con un refresh token válido.
    /// Devuelve la misma respuesta que el login, así el frontend la trata igual.
    /// </summary>
    public sealed record LoginWithRefreshTokenQuery(

        string RefreshToken

        ) : IRequest<LoginResponse>;
}
