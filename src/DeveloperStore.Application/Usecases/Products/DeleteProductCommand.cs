using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using FluentValidation;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

public sealed record DeleteProductCommand(int Id) : IRequest<Result>;

public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
    }
}

internal sealed class DeleteProductCommandHandler(IProductRepository productRepository, IUnityOfWork unityOfWork) : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var productExists = await productRepository.GetProductByIdAsync(request.Id, cancellationToken);

        if (productExists is null)
            return Result.Failure(DomainErrors.Product.ProductNotFound);

        await productRepository.DeleteProductAsync(productExists, cancellationToken);

        await unityOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}