using DeveloperStore.Presentation.Endpoints.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DeveloperStore.Presentation.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var assembly = PresentationAssembly.Get();

        ServiceDescriptor[] serviceDescriptors = assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } && type.IsAssignableTo(typeof(IEndpointMap)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpointMap), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpointMap> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpointMap>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpointMap endpointMap in endpoints)
        {
            endpointMap.MapEndpoints(builder);
        }

        return app;
    }
}