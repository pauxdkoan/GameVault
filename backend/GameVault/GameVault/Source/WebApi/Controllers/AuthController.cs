
using GameVault.Source.Application.Dtos.Auth;
using GameVault.Source.Application.Feature.Auth.Commands.RegisterUser;
using GameVault.Source.Application.Feature.Auth.Commands.RevokeRefreshToken;
using GameVault.Source.Application.Feature.Auth.Queries.Login;
using GameVault.Source.Application.Interfaces.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginUseCase _loginUseCase;
        private readonly IRegisterUseCase _registerUseCase;
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginQuery query, CancellationToken cancellationToken)
        {
           var response= await _sender.Send(query, cancellationToken);
            return Ok(response);    
        }

        [HttpPost("refresh-tokens")]
        public async Task<IActionResult> RefreshToken([FromBody] LoginWithRefreshTokenQuery query, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(query, cancellationToken);
            return Ok(response);
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
        {

            var response = await _sender.Send(command, cancellationToken);
            return Ok(response);
       
   
        }

        [HttpDelete("refresh-tokens/{id}")]
        public async Task<IActionResult> Register(Guid id, CancellationToken cancellationToken)
        {

            var succes = await _sender.Send(new RevokeRefreshTokenCommand(id), cancellationToken);
            
            return succes ? NoContent() : BadRequest();


        }





    }
}
