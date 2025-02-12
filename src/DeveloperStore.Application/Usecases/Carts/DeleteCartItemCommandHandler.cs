using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

internal sealed class DeleteCartItemCommandHandler(ICartItemsRepository cartItemsRepository, IUnityOfWork unityOfWork) : IRequestHandler<DeleteCartItemCommand, Result>
{
    public async Task<Result> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await cartItemsRepository.GetItemByIdAsync(request.CartItemId, cancellationToken);

        if (cartItem is null)
            return Result.Failure(DomainErrors.CartItem.CartItemNotFound);

        await cartItemsRepository.DeleteCartItemAsync(cartItem, cancellationToken);

        await unityOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}