using Microsoft.AspNetCore.Routing;

namespace DeveloperStore.Presentation.Endpoints.Abstractions;

internal interface IEndpointMap
{
    void MapEndpoints(IEndpointRouteBuilder app);
}
