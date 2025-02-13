using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using FluentValidation;

namespace DeveloperStore.Application.Usecases.Carts;

public sealed class AddItemToCartCommandValidator : AbstractValidator<AddItemToCartCommand>
{
    public AddItemToCartCommandValidator(ICartsRepository cartsRepository, IProductRepository productRepository)
    {
        RuleFor(p => p.CartId)
            .MustAsync(async (cartId, cancellation) =>
            {
                var cart = await cartsRepository.GetCartByIdAsync(cartId, cancellation);

                return cart is not null;
            })
            .WithErrorCode(DomainErrors.Cart.CartNotFound.Code)
            .WithMessage(DomainErrors.Cart.CartNotFound.Message);
        RuleFor(p => p.ProductId)
            .MustAsync(async (produtcId, cancellation) =>
            {
                var product = await productRepository.GetProductByIdAsync(produtcId, cancellation);

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