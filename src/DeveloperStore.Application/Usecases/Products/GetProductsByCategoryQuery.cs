using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

public sealed record GetProductsByCategoryQuery(string Category) : IRequest<Result<IEnumerable<ProductResponse>>>;

internal sealed class GetProductsByCategoryQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsByCategoryQuery, Result<IEnumerable<ProductResponse>>>
{
    public async Task<Result<IEnumerable<ProductResponse>>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetProductsByCategoryAsync(request.Category, cancellationToken);

        if (products is null || !products.Any())
            return PaginatedResult.Failure<IEnumerable<ProductResponse>>(DomainErrors.Product.ProductNotFound);

        var response = products.Select(p => new ProductResponse(p.Id, p.Title, p.Price, p.Description, p.Category, p.Image, p.Rating));

        return Result.Success(response);
    }
}