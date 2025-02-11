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

public class UpdateUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUnityOfWork _unitOfWork;
    private readonly UpdateUserCommandHandler _handler;
    private readonly Faker<User> _faker;

    public UpdateUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _unitOfWork = Substitute.For<IUnityOfWork>();
        _handler = new UpdateUserCommandHandler(_userRepository, _unitOfWork);

        _faker = new Faker<User>()
            .RuleFor(u => u.Id, f => f.Random.Number())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Password, f => f.Internet.Password())
            .RuleFor(u => u.Name, f => new Name(f.Name.FirstName(), f.Name.LastName()))
            .RuleFor(u => u.Address, f => new Address(f.Address.City(), f.Address.StreetName(), 200, f.Address.ZipCode(), f.Address.Latitude().ToString("F2"), f.Address.Longitude().ToString("F2")))
            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(u => u.Status, f => f.PickRandom<Status>())
            .RuleFor(u => u.Role, f => f.PickRandom<Role>());
    }

    [Fact]
    public async Task Handle_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var existingUser = _faker.Generate();
        var command = new UpdateUserCommand(existingUser.Id,
            "newemail@example.com",
            "NewUserName",
            "NewPassword123",
            new Name("New", "Name"),
            new Address("New Street", "New City", 200, "10001000", "+30", "-30"),
            "123456789",
            Status.Active,
            Role.Admin
        );

        _userRepository.GetUserByIdAsync(existingUser.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(existingUser));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Email, result.Value.Email);
        Assert.Equal(command.UserName, result.Value.UserName);
        Assert.Equal(command.Password, result.Value.Password);
        Assert.Equal(command.Name.FirstName, result.Value.Name.FirstName);
        Assert.Equal(command.Address.Street, result.Value.Address.Street);
        Assert.Equal(command.Phone, result.Value.Phone);
        Assert.Equal(command.Status, result.Value.Status);
        Assert.Equal(command.Role, result.Value.Role);

        await _userRepository.Received(1).UpdateUserAsync(existingUser, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new UpdateUserCommand(1,
            "newemail@example.com",
            "NewUserName",
            "NewPassword123",
            new Name("New", "Name"),
            new Address("New Street", "New City", 200, "10001000", "+30", "-30"),
            "123456789",
            Status.Active,
            Role.Admin
        );

        _userRepository.GetUserByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User>(default!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.User.UserNotFound, result.Error);

        await _userRepository.DidNotReceive().UpdateUserAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var command = new UpdateUserCommand(1,
            "newemail@example.com",
            "NewUserName",
            "NewPassword123",
            new Name("New", "Name"),
            new Address("New Street", "New City", 200, "10001000", "+30", "-30"),
            "123456789",
            Status.Active,
            Role.Admin
        );

        _userRepository.GetUserByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<User>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
