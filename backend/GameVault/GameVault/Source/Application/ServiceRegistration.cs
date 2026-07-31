using GameVault.Source.Application.Behaviors;
using GameVault.Source.Application.Interfaces.Auth;
using GameVault.Source.Application.Interfaces.Security;
using GameVault.Source.Application.Services.Security;
using System.Reflection;
using FluentValidation;


namespace GameVault.Source.Application
{
    public static class ServiceRegistration
    {

        public static void AddApplicationLayerForWebApi(this IServiceCollection services)
        {
            GenericServices(services);
            GenericConfigurations(services);
        }

        private static void GenericServices(IServiceCollection services)
        {
            services.AddTransient<ITokenProvider, TokenProvider>();
       
        }

        private static void GenericConfigurations(this IServiceCollection service)
        {

            var assembly = Assembly.GetExecutingAssembly();

            service.AddAutoMapper(cfg => { }, assembly);

            service.AddValidatorsFromAssembly(assembly);

            service.AddMediatR(cfg => {

                cfg.RegisterServicesFromAssembly(assembly);

                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                
            });
        }
    }
}
