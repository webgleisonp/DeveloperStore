using DeveloperStore.Application.Usecases.Users;
using DeveloperStore.Domain.Shared;
using DeveloperStore.Presentation.Endpoints.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DeveloperStore.Presentation.Endpoints.MapEndpoints.Users;

internal sealed class MapUsersEndpoint : IEndpointMap
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("users", GetUsersAsync)
            .Produces<PaginatedResult<IEnumerable<UserResponse>>>()
            .Produces(404)
            .Produces<PaginatedResult<IEnumerable<UserResponse>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Users")
            .MapToApiVersion(1);

        app.MapGet("users/{id}", GetUserByIdAsync)
            .Produces<Result<UserResponse>>()
            .Produces(404)
            .Produces<Result<UserResponse>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Users")
            .MapToApiVersion(1);

        app.MapPost("users", CreateUserAsync)
            .Produces<Result<UserResponse>>(StatusCodes.Status201Created)
            .Produces<Result<UserResponse>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Users")
            .MapToApiVersion(1);

        app.MapPut("users/{id}", UpdateUserAsync)
            .Produces<Result<UserResponse>>()
            .Produces(404)
            .Produces<Result<UserResponse>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Users")
            .MapToApiVersion(1);

        app.MapDelete("users/{id}", DeleteUserAsync)
            .Produces<Result<UserResponse>>()
            .Produces(404)
            .Produces<Result<UserResponse>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Users")
            .MapToApiVersion(1);
    }

    public async Task<IResult> GetUsersAsync(ISender sender,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? order,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery(page, pageSize, order);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> GetUserByIdAsync(ISender sender,
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> CreateUserAsync(ISender sender,
        [FromBody] UserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.UserName,
            request.Password,
            request.Name,
            request.Address,
            request.Phone,
            request.Status,
            request.Role);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return Results.Created("/products", result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> UpdateUserAsync(ISender sender,
        [FromRoute] int id,
        [FromBody] UserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(id, request.Email, request.UserName, request.Password, request.Name, request.Address, request.Phone, request.Status, request.Role);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> DeleteUserAsync(ISender sender,
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }
}
