using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using FluentValidation;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

public sealed class CreateUserCommandHandler(IValidator<CreateUserCommand> validator, IUserRepository userRepository, IUnityOfWork unitOfWork) : IRequestHandler<CreateUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => new Error(
                    error.PropertyName,
                    error.ErrorMessage))
                .ToArray();

            return Result.Failure<UserResponse>(errors);
        }

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