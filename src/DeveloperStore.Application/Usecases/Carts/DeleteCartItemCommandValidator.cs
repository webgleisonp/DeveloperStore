using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using FluentValidation;

namespace DeveloperStore.Application.Usecases.Carts;

public sealed class DeleteCartItemCommandValidator : AbstractValidator<DeleteCartItemCommand>
{
    public DeleteCartItemCommandValidator(ICartsRepository cartsRepository, ICartItemsRepository cartItemsRepository)
    {
        RuleFor(p => p.CartId)
            .NotEmpty()
            .MustAsync(async (cartId, cancellation) =>
            {
                var cart = await cartsRepository.GetCartByIdAsync(cartId, cancellation);

                return cart is not null;
            })
            .WithErrorCode(DomainErrors.Cart.CartNotFound.Code)
            .WithMessage(DomainErrors.Cart.CartNotFound.Message);

        RuleFor(p => p.CartItemId)
            .NotEmpty()
            .MustAsync(async (cartItemId, cancellation) =>
            {
                var cartItem = await cartItemsRepository.GetItemByIdAsync(cartItemId, cancellation);

                return cartItem is not null;
            })
            .WithErrorCode(DomainErrors.CartItem.CartItemNotFound.Code)
            .WithMessage(DomainErrors.CartItem.CartItemNotFound.Message);
    }
}