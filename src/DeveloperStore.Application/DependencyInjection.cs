using DeveloperStore.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DeveloperStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(ApplicationAssembly.Get());

            conf.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Registra todos os validadores do assembly
        services.AddValidatorsFromAssembly(ApplicationAssembly.Get());

        return services;
    }
}