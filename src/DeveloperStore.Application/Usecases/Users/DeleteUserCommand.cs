using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using FluentValidation;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

public sealed record DeleteUserCommand(int Id) : IRequest<Result<UserResponse>>;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

internal sealed class DeleteUserCommandHandler(IValidator<DeleteUserCommand> validator, IUserRepository userRepository, IUnityOfWork unityOfWork) : IRequestHandler<DeleteUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
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

        var userExists = await userRepository.GetUserByIdAsync(request.Id, cancellationToken);

        if (userExists is null)
            return Result.Failure<UserResponse>(DomainErrors.User.UserNotFound);

        await userRepository.DeleteUserAsync(userExists, cancellationToken);

        await unityOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserResponse(userExists.Id, userExists.Email, userExists.UserName, userExists.Password, userExists.Name, userExists.Address, userExists.Phone, userExists.Status, userExists.Role));
    }
}