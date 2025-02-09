using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

internal sealed class UpdateProductCommandHandler(IProductRepository productRepository, IUnityOfWork unityOfWork) : IRequestHandler<UpdateProductCommand, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var productExists = await productRepository.GetProductByIdAsync(request.Id, cancellationToken);

        if (productExists is null)
            return Result.Failure<ProductResponse>(DomainErrors.Product.ProductNotFound);

        productExists.Title = request.Title;
        productExists.Price = request.Price;
        productExists.Description = request.Description;
        productExists.Category = request.Category;
        productExists.Image = request.Image;
        productExists.Rating = request.Rating;

        await productRepository.UpdateProductAsync(productExists, cancellationToken);

        await unityOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ProductResponse(productExists.Id, productExists.Title, productExists.Price, productExists.Description, productExists.Category, productExists.Image, productExists.Rating));
    }
}