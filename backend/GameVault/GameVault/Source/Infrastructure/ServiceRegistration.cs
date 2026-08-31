using GameVault.Source.Application.Interfaces.Igdb;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Domain.Settings;
using GameVault.Source.Infrastructure.Contexts;
using GameVault.Source.Infrastructure.ExternalServices.Igdb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GameVault.Source.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastucture(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureContex(services, configuration);
            ConfigureIdentity(services);
            ConfigureJwt(services, configuration);
            GenericConfigurations(services, configuration);
            ExternalServices(services);
        }

        private static void ConfigureContex(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GameVaultContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    m => m.MigrationsAssembly(typeof(GameVaultContext).Assembly.FullName));
            });
        }

        private static void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<GameVaultContext>()
            .AddDefaultTokenProviders();
        }

        private static void ConfigureJwt(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

            if (jwtSettings is null)
            {
                throw new InvalidOperationException("JwtSettings no está configurado.");
            }

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });
        }

        private static void GenericConfigurations(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IgdbSettings>(configuration.GetSection(IgdbSettings.SectionName));
        }

        private static void ExternalServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddHttpClient<IIgdbAccessTokenProvider, IgdbAccessTokenProvider>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            services.AddHttpClient<IIgdbApiClient, IgdbApiClient>((serviceProvider, client) =>
            {
                var settings = serviceProvider
                    .GetRequiredService<IOptions<IgdbSettings>>().Value;

                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(15);
            });
        }
    }
}
