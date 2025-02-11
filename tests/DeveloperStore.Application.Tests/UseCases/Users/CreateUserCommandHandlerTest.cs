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

public class CreateUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUnityOfWork _unitOfWork;
    private readonly CreateUserCommandHandler _handler;
    private readonly Faker _faker;

    public CreateUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _unitOfWork = Substitute.For<IUnityOfWork>();
        _handler = new CreateUserCommandHandler(_userRepository, _unitOfWork);
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenEmailDoesNotExist()
    {
        // Arrange
        var command = new CreateUserCommand(
            _faker.Internet.Email(),
            _faker.Internet.UserName(),
            _faker.Internet.Password(),
            new Name(
                _faker.Person.FirstName,
                _faker.Person.LastName
            ),
            new Address(
                _faker.Address.City(),
                _faker.Address.StreetName(),
                440,
                _faker.Address.ZipCode(),
                _faker.Address.Latitude().ToString("F2"),
                _faker.Address.Longitude().ToString("F2")
            ),
            _faker.Phone.PhoneNumber(),
            Status.Active,
            Role.Admin
        );

        _userRepository.GetUserByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(Task.FromResult<User>(default!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(command.Email, result.Value.Email);
        await _userRepository.Received(1).CreateNewUserAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailAlreadyExists()
    {
        // Arrange
        var existingUser = new User { Email = _faker.Internet.Email() };
        var command = new CreateUserCommand(
            existingUser.Email,
            _faker.Internet.UserName(),
            _faker.Internet.Password(),
            new Name(
                _faker.Person.FirstName,
                _faker.Person.LastName
            ),
            new Address(
                _faker.Address.City(),
                _faker.Address.StreetName(),
                440,
                _faker.Address.ZipCode(),
                _faker.Address.Latitude().ToString("F2"),
                _faker.Address.Longitude().ToString("F2")
            ),
            _faker.Phone.PhoneNumber(),
            Status.Active,
            Role.Admin
        );

        _userRepository.GetUserByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(Task.FromResult(existingUser));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.User.UserExists, result.Error);
        await _userRepository.DidNotReceive().CreateNewUserAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var command = new CreateUserCommand(
            _faker.Internet.Email(),
            _faker.Internet.UserName(),
            _faker.Internet.Password(),
            new Name(
                _faker.Person.FirstName,
                _faker.Person.LastName
            ),
            new Address(
                _faker.Address.City(),
                _faker.Address.StreetName(),
                440,
                _faker.Address.ZipCode(),
                _faker.Address.Latitude().ToString("F2"),
                _faker.Address.Longitude().ToString("F2")
            ),
            _faker.Phone.PhoneNumber(),
            Status.Active,
            Role.Admin
        );

        _userRepository.GetUserByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(Task.FromResult<User>(default!));
        _userRepository.CreateNewUserAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
