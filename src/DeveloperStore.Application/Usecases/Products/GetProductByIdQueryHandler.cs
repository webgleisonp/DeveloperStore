using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

internal sealed class GetProductByIdQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var productExists = await productRepository.GetProductByIdAsync(request.Id, cancellationToken);

        if (productExists is null)
            return Result.Failure<ProductResponse>(DomainErrors.Product.ProductNotFound);

        return Result.Success(new ProductResponse(productExists.Id, productExists.Title, productExists.Price, productExists.Description, productExists.Category, productExists.Image, productExists.Rating));
    }
}