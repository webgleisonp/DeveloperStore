﻿using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

internal sealed class UpdateUserCommandHandler(IUserRepository userRepository, IUnityOfWork unityOfWork) : IRequestHandler<UpdateUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await userRepository.GetUserByIdAsync(request.Id, cancellationToken);

        if (userExists is null)
            return Result.Failure<UserResponse>(DomainErrors.User.UserNotFound);

        userExists.Email = request.Email;
        userExists.UserName = request.UserName;
        userExists.Password = request.Password;
        userExists.Name = request.Name;
        userExists.Address = request.Address;
        userExists.Phone = request.Phone;
        userExists.Status = request.Status;
        userExists.Role = request.Role;

        await userRepository.UpdateUserAsync(userExists, cancellationToken);

        await unityOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserResponse(userExists.Id, userExists.Email, userExists.UserName, userExists.Password, userExists.Name, userExists.Address, userExists.Phone, userExists.Status, userExists.Role));
    }
}