using GameVault.Source.Application.Feature.Auth;
using GameVault.Source.Application.Interfaces.Auth;
using GameVault.Source.Application.Interfaces.Security;
using GameVault.Source.Application.Services.Security;

namespace GameVault.Source.Application
{
    public static class ServiceRegistration
    {

        public static void AddApplicationLayerForWebApi(this IServiceCollection services)
        {
            GenericServices(services);

        }

        private static void GenericServices(IServiceCollection services)
        {
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddTransient<ILoginUseCase, LoginUseCase>();

        }
    }
}
