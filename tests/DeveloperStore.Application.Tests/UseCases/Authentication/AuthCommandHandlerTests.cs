using Bogus;
using DeveloperStore.Application.Abstractions;
using DeveloperStore.Application.Usecases.Authentication;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Authentication;

public class AuthCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IJwtProvider _jwtProvider = Substitute.For<IJwtProvider>();
    private readonly AuthCommandHandler _handler;

    public AuthCommandHandlerTests()
    {
        _handler = new AuthCommandHandler(_userRepository, _jwtProvider);
    }

    private static Faker<User> UserFaker =>
        new Faker<User>()
            .RuleFor(u => u.Id, f => f.Random.Int(1, 1000))
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Password, f => f.Internet.Password());

    [Fact]
    public async Task AuthCommandHandler_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var fakeUser = UserFaker.Generate();
        var fakeToken = "fake-jwt-token";

        _userRepository.GetUserByUserNameAndPasswordAsync(fakeUser.UserName, fakeUser.Password, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User>(fakeUser));

        _jwtProvider.Generate(fakeUser).Returns(fakeToken);

        var command = new AuthCommand(fakeUser.UserName, fakeUser.Password);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(fakeToken, result.Value.Token);
        _jwtProvider.Received(1).Generate(fakeUser);
    }

    [Fact]
    public async Task AuthCommandHandler_ShouldReturnFailure_WhenCredentialsAreInvalid()
    {
        // Arrange
        _userRepository.GetUserByUserNameAndPasswordAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(default)!);

        var command = new AuthCommand("invalidUser", "invalidPass");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.User.InvalidCredentials, result.Error);
        _jwtProvider.DidNotReceiveWithAnyArgs().Generate(Arg.Any<User>());
    }
}
