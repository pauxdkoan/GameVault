using GameVault.Source.Application.Dtos.Auth;
using GameVault.Source.Application.Dtos.User;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace GameVault.Source.Application.Feature.Auth.Queries.Login
{
    public sealed record LoginWithRefreshTokenQuery(

        string RefreshToken

        ) : IRequest<Response>;
   
    public sealed record Response( 

        string AccessToken, string RefrehToken
     );
}
