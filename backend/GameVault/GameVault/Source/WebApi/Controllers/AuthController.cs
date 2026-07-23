
using GameVault.Source.Application.Dtos.Auth;
using GameVault.Source.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginUseCase _loginUseCase;

        public AuthController(ILoginUseCase loginUseCase)
        {
            _loginUseCase = loginUseCase;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var response = await _loginUseCase.ExecuteAsync(request);
                return Ok(response);
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message=ex.Message,
                });
            }
        }
    }
}
