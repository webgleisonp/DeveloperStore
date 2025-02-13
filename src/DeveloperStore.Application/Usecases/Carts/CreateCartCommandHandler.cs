using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

internal sealed class CreateCartCommandHandler(ICartsRepository cartsRepository, ICartItemsRepository cartItemsRepository, IUnityOfWork unityOfWork) : IRequestHandler<CreateCartCommand, Result<CartsResponse>>
{
    public async Task<Result<CartsResponse>> Handle(CreateCartCommand request, CancellationToken cancellationToken)
    {
        var cartExists = await cartsRepository.GetCartByUserIdAsync(request.UserId, cancellationToken);

        if (cartExists is not null)
            return Result.Failure<CartsResponse>(DomainErrors.Cart.CartExists);

        var newCart = new Cart
        {
            UserId = request.UserId,
            CreateDate = request.CreateDate,
            Active = true,
        };

        foreach (var cartItem in request.CartItems)
        {
            var newCartItem = new CartItem
            {
                Cart = newCart,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                Price = cartItem.ItemPrice
            };

            await cartItemsRepository.CreateCartItemAsync(newCartItem, cancellationToken);

            newCart.CartItems.Add(newCartItem);
        }

        await unityOfWork.SaveChangesAsync(cancellationToken);

        var cartItemsResponse = newCart.CartItems.Select(ci => new CartItemsResponse(ci.CartId, ci.ProductId, ci.Quantity, ci.Price));

        return Result.Success(new CartsResponse(newCart.Id, newCart.UserId, newCart.CreateDate, cartItemsResponse));
    }
}
