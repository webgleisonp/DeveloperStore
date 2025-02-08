using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

internal sealed class GetUserByIdQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userExists = await userRepository.GetUserByIdAsync(request.Id, cancellationToken);

        if (userExists is null)
            return Result.Failure<UserResponse>(DomainErrors.User.UserNotFound);

        return Result.Success(new UserResponse(userExists.Id, userExists.Email, userExists.UserName, userExists.Password, userExists.Name, userExists.Address, userExists.Phone, userExists.Status, userExists.Role));
    }
}