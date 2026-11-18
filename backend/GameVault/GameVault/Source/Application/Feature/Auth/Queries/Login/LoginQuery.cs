using GameVault.Source.Application.Dtos.Auth;
using GameVault.Source.Application.Dtos.User;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace GameVault.Source.Application.Feature.Auth.Queries.Login
{
    public sealed record LoginQuery(

        string Email,
        string Password

        ) : IRequest<LoginResponse>;
   
    public sealed record LoginResponse( 

     string AccessToken,
     string RefreshToken,
     DateTime AccessTokenExpiresAt,
     UserDto User
     );
}
