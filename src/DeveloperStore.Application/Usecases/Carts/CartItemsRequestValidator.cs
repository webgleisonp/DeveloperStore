using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using FluentValidation;

namespace DeveloperStore.Application.Usecases.Carts;

public sealed class CartItemsRequestValidator : AbstractValidator<CartItemsRequest>
{
    public CartItemsRequestValidator(IProductRepository productRepository)
    {
        RuleFor(p => p.ProductId)
            .NotEmpty()
            .MustAsync(async (productId, cancelation) =>
            {
                var product = await productRepository.GetProductByIdAsync(productId, cancelation);

                return product is not null;
            })
            .WithErrorCode(DomainErrors.Product.ProductNotFound.Code)
            .WithMessage(DomainErrors.Product.ProductNotFound.Message);
        RuleFor(p => p.Quantity)
            .GreaterThan(0)
            .LessThanOrEqualTo(20);
        RuleFor(p => p.ItemPrice).GreaterThan(0);
    }
}