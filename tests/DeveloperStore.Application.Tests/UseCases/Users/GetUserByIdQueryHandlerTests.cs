using Bogus;
using DeveloperStore.Application.Usecases.Users;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.ValueObjects;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Users;

public class GetUserByIdQueryHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly GetUserByIdQueryHandler _handler;
    private readonly Faker _faker;

    public GetUserByIdQueryHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new GetUserByIdQueryHandler(_userRepository);
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public async Task GetUserByIdQueryHandler_ShouldReturnUser_WhenUserExists()
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

        var query = new GetUserByIdQuery(user.Id);

        _userRepository.GetUserByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(user));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(user.Id, result.Value.Id);
        Assert.Equal(user.Email, result.Value.Email);
    }

    [Fact]
    public async Task GetUserByIdQueryHandler_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var query = new GetUserByIdQuery(1);

        _userRepository.GetUserByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User>(default!));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.User.UserNotFound, result.Error);
    }

    [Fact]
    public async Task GetUserByIdQueryHandler_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var query = new GetUserByIdQuery(1);

        _userRepository.GetUserByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<User>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }
}
