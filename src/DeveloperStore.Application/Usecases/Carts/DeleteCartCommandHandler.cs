using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

internal sealed class DeleteCartCommandHandler(ICartsRepository cartsRepository, IUnityOfWork unityOfWork) : IRequestHandler<DeleteCartCommand, Result>
{
    public async Task<Result> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
    {
        var cartExists = await cartsRepository.GetCartByIdAsync(request.Id, cancellationToken);

        if (cartExists is null)
            return Result.Failure<CartsResponse>(DomainErrors.Cart.CartNotFound);

        await cartsRepository.DeleteCartAsync(cartExists, cancellationToken);

        await unityOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
