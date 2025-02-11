using Bogus;
using DeveloperStore.Application.Abstractions;
using DeveloperStore.Application.Usecases.Users;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.ValueObjects;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Users;

public class DeleteUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUnityOfWork _unityOfWork;
    private readonly DeleteUserCommandHandler _handler;
    private readonly Faker _faker;

    public DeleteUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _unityOfWork = Substitute.For<IUnityOfWork>();
        _handler = new DeleteUserCommandHandler(_userRepository, _unityOfWork);
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = _faker.Internet.Email(),
            UserName = _faker.Internet.UserName(),
            Password = _faker.Internet.Password(),
            Name = new Name(
                _faker.Person.FirstName,
                _faker.Person.LastName
            ),
            Address = new Address(
                _faker.Address.City(),
                _faker.Address.StreetName(),
                440,
                _faker.Address.ZipCode(),
                _faker.Address.Latitude().ToString("F2"),
                _faker.Address.Longitude().ToString("F2")
            ),
            Phone = _faker.Phone.PhoneNumber(),
            Status = Status.Active,
            Role = Role.Admin
        };

        var command = new DeleteUserCommand(user.Id);

        _userRepository.GetUserByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(user));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(user.Id, result.Value.Id);
        await _userRepository.Received(1).DeleteUserAsync(user, Arg.Any<CancellationToken>());
        await _unityOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new DeleteUserCommand(1);

        _userRepository.GetUserByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User>(default!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.User.UserNotFound, result.Error);
        await _userRepository.DidNotReceive().DeleteUserAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unityOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = _faker.Internet.Email(),
            UserName = _faker.Internet.UserName(),
            Password = _faker.Internet.Password(),
            Name = new Name(
                _faker.Person.FirstName,
                _faker.Person.LastName
            ),
            Address = new Address(
                _faker.Address.City(),
                _faker.Address.StreetName(),
                440,
                _faker.Address.ZipCode(),
                _faker.Address.Latitude().ToString("F2"),
                _faker.Address.Longitude().ToString("F2")
            ),
            Phone = _faker.Phone.PhoneNumber(),
            Status = Status.Active,
            Role = Role.Admin
        };

        var command = new DeleteUserCommand(user.Id);

        _userRepository.GetUserByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(user));

        _userRepository.DeleteUserAsync(user, Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
