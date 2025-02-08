using DeveloperStore.Application.Usecases.Authentication;
using DeveloperStore.Domain.Shared;
using DeveloperStore.Presentation.Endpoints.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DeveloperStore.Presentation.Endpoints.MapEndpoints.Auth;

internal sealed class MapAuthEndpoint : IEndpointMap
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("auth", AuthenticateUserAsync)
            .Produces<Result<AuthResponse>>()
            .Produces(404)
            .Produces<Result<AuthResponse>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Auth")
            .MapToApiVersion(1);
    }

    public async Task<IResult> AuthenticateUserAsync(ISender sender,
        [FromBody] AuthRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AuthCommand(request.UserName, request.Password);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            var problemDetails = ProblemDetailsFactory.CreateValidationProblemDetails(
                result.Errors,
                "/auth");

            return Results.BadRequest(problemDetails);
        }

        return Results.Ok(result.Value);
    }
}
