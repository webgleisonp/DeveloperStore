using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

internal sealed class DeleteUserCommandHandler(IUserRepository userRepository, IUnityOfWork unityOfWork) : IRequestHandler<DeleteUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await userRepository.GetUserByIdAsync(request.Id, cancellationToken);

        if (userExists is null)
            return Result.Failure<UserResponse>(DomainErrors.User.UserNotFound);

        await userRepository.DeleteUserAsync(userExists, cancellationToken);

        await unityOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserResponse(userExists.Id, userExists.Email, userExists.UserName, userExists.Password, userExists.Name, userExists.Address, userExists.Phone, userExists.Status, userExists.Role));
    }
}