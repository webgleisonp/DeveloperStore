using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Authentication;

internal sealed class AuthCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider) : IRequestHandler<AuthCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(AuthCommand request, CancellationToken cancellationToken)
    {
        var userExists = await userRepository.GetUserByUserNameAndPasswordAsync(request.UserName, request.Password, cancellationToken);

        if (userExists is null)
            return Result.Failure<AuthResponse>(DomainErrors.User.InvalidCredentials);

        var token = jwtProvider.Generate(userExists);

        return Result.Success(new AuthResponse(token));
    }
}