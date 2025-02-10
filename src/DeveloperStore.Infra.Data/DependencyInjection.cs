using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeveloperStore.Infra.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<DeveloperStoreDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(DeveloperStoreDbContext).Assembly.FullName));
        });

        services.AddScoped<IUnityOfWork, UnityOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartsRepository, CartsRepository>();
        services.AddScoped<ICartItemsRepository, CartItemsRepository>();

        return services;
    }
} 