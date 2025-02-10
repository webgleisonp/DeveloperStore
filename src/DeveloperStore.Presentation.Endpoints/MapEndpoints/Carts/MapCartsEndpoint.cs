using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Shared;
using DeveloperStore.Presentation.Endpoints.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DeveloperStore.Presentation.Endpoints.MapEndpoints.Carts;

internal sealed class MapCartsEndpoint : IEndpointMap
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("carts", GetCartsAsync)
            .Produces<PaginatedResult<IEnumerable<CartsResponse>>>()
            .Produces(404)
            .Produces<PaginatedResult<IEnumerable<CartsResponse>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Carts")
            .MapToApiVersion(1);

        app.MapGet("carts/{id}", GetCartByIdAsync)
            .Produces<PaginatedResult<IEnumerable<CartsResponse>>>()
            .Produces(404)
            .Produces<PaginatedResult<IEnumerable<CartsResponse>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Carts")
            .MapToApiVersion(1);

        app.MapPost("carts", CreateCartAsync)
            .Produces<Result<IEnumerable<CartsResponse>>>(StatusCodes.Status201Created)
            .Produces<Result<IEnumerable<CartsResponse>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Carts")
            .MapToApiVersion(1);

        app.MapPut("carts/{id}/cart-items", AddItemToCartAsync)
            .Produces<PaginatedResult<IEnumerable<CartsResponse>>>()
            .Produces(404)
            .Produces<PaginatedResult<IEnumerable<CartsResponse>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Carts")
            .MapToApiVersion(1);

        app.MapDelete("carts/{id}", DeleteCartAsync)
            .Produces<PaginatedResult<IEnumerable<CartsResponse>>>()
            .Produces(404)
            .Produces<PaginatedResult<IEnumerable<CartsResponse>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Carts")
            .MapToApiVersion(1);

        app.MapDelete("carts/{id}/cart-items/{itemId}", DeleteCartItemAsync)
            .Produces<PaginatedResult<IEnumerable<CartsResponse>>>()
            .Produces(404)
            .Produces<PaginatedResult<IEnumerable<CartsResponse>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Carts")
            .MapToApiVersion(1);
    }

    public async Task<IResult> GetCartsAsync(ISender sender,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? order,
        CancellationToken cancellationToken)
    {
        var query = new GetCartsQuery(page, pageSize, order);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> GetCartByIdAsync(ISender sender,
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var query = new GetCartByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> CreateCartAsync(ISender sender,
        [FromBody] CartsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCartCommand(request.UserId, request.CreateDate, request.CartItems);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> AddItemToCartAsync(ISender sender,
        [FromRoute] int id,
        [FromBody] CartItemsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddItemToCartCommand(id, request.ProductId, request.Quantity, request.ItemPrice);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> DeleteCartAsync(ISender sender,
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCartCommand(id);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> DeleteCartItemAsync(ISender sender,
        [FromRoute] int id,
        [FromRoute] int itemId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCartItemCommand(id, itemId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }
}
