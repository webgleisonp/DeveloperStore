using DeveloperStore.Application.Usecases.Products;
using DeveloperStore.Domain.Shared;
using DeveloperStore.Presentation.Endpoints.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DeveloperStore.Presentation.Endpoints.MapEndpoints.Products;

internal sealed class MapProductsEndpoint : IEndpointMap
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("products", GetProductsAsync)
            .Produces<PaginatedResult<IEnumerable<ProductResponse>>>()
            .Produces(404)
            .Produces<PaginatedResult<IEnumerable<ProductResponse>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Products")
            .MapToApiVersion(1);

        app.MapGet("products/{id}", GetProductByIdAsync)
            .Produces<Result<ProductResponse>>()
            .Produces(404)
            .Produces<Result<ProductResponse>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Products")
            .MapToApiVersion(1);

        app.MapPost("products", CreateProductsAsync)
            .Produces<Result<ProductResponse>>(StatusCodes.Status201Created)
            .Produces<Result<ProductResponse>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Products")
            .MapToApiVersion(1);

        app.MapPut("products/{id}", UpdateProductAsync)
            .Produces<Result<ProductResponse>>()
            .Produces(404)
            .Produces<Result<ProductResponse>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Products")
            .MapToApiVersion(1);

        app.MapDelete("products/{id}", DeleteProductAsync)
            .Produces<Result<ProductResponse>>()
            .Produces(404)
            .Produces<Result<ProductResponse>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Products")
            .MapToApiVersion(1);

        app.MapGet("products/categories", GetProductsCategoriesAsync)
            .Produces<PaginatedResult<IEnumerable<string>>>()
            .Produces(404)
            .Produces<PaginatedResult<IEnumerable<string>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Products")
            .MapToApiVersion(1);

        app.MapGet("products/category/{category}", GetProductsByCategoryAsync)
            .Produces<PaginatedResult<IEnumerable<ProductResponse>>>()
            .Produces(404)
            .Produces<PaginatedResult<IEnumerable<ProductResponse>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Products")
            .MapToApiVersion(1);
    }

    public async Task<IResult> GetProductsAsync(ISender sender,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? order,
        CancellationToken cancellationToken)
    {
        var query = new GetProductsQuery(page, pageSize, order);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> GetProductByIdAsync(ISender sender,
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> CreateProductsAsync(ISender sender,
        [FromBody] ProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Title,
            request.Price,
            request.Description,
            request.Category,
            request.Image,
            request.Rating);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Created("/products", result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> UpdateProductAsync(ISender sender,
        [FromRoute] int id,
        [FromBody] ProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Title,
            request.Price,
            request.Description,
            request.Category,
            request.Image,
            request.Rating);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> DeleteProductAsync(ISender sender,
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> GetProductsCategoriesAsync(ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetProductsCategoriesQuery();

        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }

    public async Task<IResult> GetProductsByCategoryAsync(ISender sender, [FromRoute] string category, CancellationToken cancellationToken)
    {
        var query = new GetProductsByCategoryQuery(category);

        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure && result.Error.Code == "404")
            return Results.NotFound(result);

        if (result.IsSuccess)
            return Results.Ok(result);

        return Results.BadRequest(result);
    }
}
