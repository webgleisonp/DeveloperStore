using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

internal sealed class CreateProductCommandHandler(IProductRepository productRepository, IUnityOfWork unityOfWork) : IRequestHandler<CreateProductCommand, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productExists = await productRepository.GetProductsByTitleAsync(request.Title, cancellationToken);

        if (productExists is not null)
            return Result.Failure<ProductResponse>(DomainErrors.Product.ProductExists);

        var newProduct = new Product
        {
            Title = request.Title,
            Price = request.Price,
            Description = request.Description,
            Category = request.Category,
            Image = request.Image,
            Rating = request.Rating
        };

        await productRepository.CreateProductsAsync(newProduct, cancellationToken);

        await unityOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ProductResponse(newProduct.Id, newProduct.Title, newProduct.Price, newProduct.Description, newProduct.Category, newProduct.Image, newProduct.Rating));
    }
}