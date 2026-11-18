using GameVault.Source.Application.Dtos.User;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace GameVault.Source.Application.Feature.Auth.Commands.RegisterUser
{
    public sealed record RegisterUserCommand(

        string Name,
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword
        ) : IRequest<UserDto>;
   
}
