using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

internal sealed class DeleteCartItemCommandHandler(ICartItemsRepository cartItemsRepository, IUnityOfWork unityOfWork) : IRequestHandler<DeleteCartItemCommand, Result>
{
    public async Task<Result> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await cartItemsRepository.GetItemByIdAsync(request.CartItemId, cancellationToken);

        await cartItemsRepository.DeleteCartItemAsync(cartItem, cancellationToken);

        await unityOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}