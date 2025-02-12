using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

internal sealed class AddItemToCartCommandHandler(ICartsRepository cartRepository, ICartItemsRepository cartItemsRepository, IUnityOfWork unityOfWork) : IRequestHandler<AddItemToCartCommand, Result<CartsResponse>>
{
    public async Task<Result<CartsResponse>> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        var cartItemExists = await cartItemsRepository.GetCartItemByProductIdAsync(request.CartId, request.ProductId, cancellationToken);

        if (cartItemExists is not null)
            return Result.Failure<CartsResponse>(DomainErrors.CartItem.CartItemExists);

        var newCartItem = new CartItem
        {
            CartId = request.CartId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Price = request.ItemPrice
        };

        await cartItemsRepository.CreateCartItemAsync(newCartItem, cancellationToken);

        await unityOfWork.SaveChangesAsync(cancellationToken);

        var cartResult = await cartRepository.GetCartByIdAsync(request.CartId, cancellationToken);

        if (cartResult is null)
            return Result.Failure<CartsResponse>(DomainErrors.Cart.CartNotFound);

        var cartItemsResult = await cartItemsRepository.GetItemsByCartIdAsync(request.CartId, cancellationToken);

        if (cartItemsResult.Count() == 0)
            return Result.Failure<CartsResponse>(DomainErrors.CartItem.CartItemNotFound);

        return Result.Success(new CartsResponse(cartResult.Id,
            cartResult.UserId,
            cartResult.CreateDate,
            cartItemsResult.Select(ci => new CartItemsResponse(ci.CartId, ci.ProductId, ci.Quantity, ci.Price))));
    }
}
