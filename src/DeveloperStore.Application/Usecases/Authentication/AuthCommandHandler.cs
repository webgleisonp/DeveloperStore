using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using FluentValidation;
using MediatR;

namespace DeveloperStore.Application.Usecases.Authentication;

internal sealed class AuthCommandHandler(IValidator<AuthCommand> validator, IUserRepository userRepository, IJwtProvider jwtProvider) : IRequestHandler<AuthCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(AuthCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => new Error(
                    error.PropertyName,
                    error.ErrorMessage))
                .ToArray();

            return Result.Failure<AuthResponse>(errors);
        }

        var userExists = await userRepository.GetUserByUserNameAndPasswordAsync(request.UserName, request.Password, cancellationToken);

        if (userExists is null)
            return Result.Failure<AuthResponse>(DomainErrors.User.InvalidCredentials);

        var token = jwtProvider.Generate(userExists);

        return Result.Success(new AuthResponse(token));
    }
}