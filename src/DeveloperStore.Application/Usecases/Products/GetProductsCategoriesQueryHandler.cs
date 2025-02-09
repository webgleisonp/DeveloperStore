using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

internal sealed class GetProductsCategoriesQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsCategoriesQuery, Result<IEnumerable<string>>>
{
    public async Task<Result<IEnumerable<string>>> Handle(GetProductsCategoriesQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetProductsAsync(cancellationToken);

        if (products is null || !products.Any())
            return PaginatedResult.Failure<IEnumerable<string>>(DomainErrors.Product.ProductsTableIsEmpty);

        var result = products.Select(p => p.Category).Distinct();

        return Result.Success(result);
    }
}