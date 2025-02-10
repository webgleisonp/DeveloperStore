using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

internal sealed class GetCartByIdQueryHandler(ICartsRepository cartsRepository, ICartItemsRepository cartItemsRepository) : IRequestHandler<GetCartByIdQuery, Result<CartsResponse>>
{
    public async Task<Result<CartsResponse>> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
    {
        var cartExists = await cartsRepository.GetCartByIdAsync(request.Id, cancellationToken);

        if (cartExists is null)
            return Result.Failure<CartsResponse>(DomainErrors.Cart.CartNotFound);

        var cartItems = await cartItemsRepository.GetItemsByCartIdAsync(request.Id, cancellationToken);

        var itemsResult = cartItems.Select(ci => new CartItemsResponse(ci.CartId, ci.ProductId, ci.Quantity, ci.Price));

        return Result.Success(new CartsResponse(cartExists.Id, cartExists.UserId, cartExists.CreateDate, itemsResult));
    }
}
