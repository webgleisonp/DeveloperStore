using DeveloperStore.Application.Abstractions;
using DeveloperStore.Infra.Security.Authentication;
using DeveloperStore.Infra.Security.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DeveloperStore.Infra.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = configuration.GetSection("Jwt:Issuer").Value,
                ValidAudience = configuration.GetSection("Jwt:Audience").Value,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:SecretKey").Value!))
            });

        services.AddAuthorization();

        services.AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }
}