﻿using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

internal sealed class CreateUserCommandHandler(IUserRepository userRepository, IUnityOfWork unitOfWork) : IRequestHandler<CreateUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await userRepository.GetUserByEmailAsync(request.Email, cancellationToken);

        if (userExists is not null)
            return Result.Failure<UserResponse>(DomainErrors.User.UserExists);

        var newUser = new User
        {
            Email = request.Email,
            UserName = request.UserName,
            Password = request.Password,
            Name = request.Name,
            Address = request.Address,
            Phone = request.Phone,
            Status = request.Status,
            Role = request.Role
        };

        await userRepository.CreateNewUserAsync(newUser, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserResponse(newUser.Id, newUser.Email, newUser.UserName, newUser.Password, newUser.Name, newUser.Address, newUser.Phone, newUser.Status, newUser.Role));
    }
}