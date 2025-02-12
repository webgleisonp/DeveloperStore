using Bogus;
using DeveloperStore.Application.Usecases.Users;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.ValueObjects;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Users;

public class GetUsersQueryHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly GetUsersQueryHandler _handler;
    private readonly Faker<User> _faker;

    public GetUsersQueryHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new GetUsersQueryHandler(_userRepository);

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
    public async Task GetUsersQueryHandler_ShouldReturnPaginatedUsers_WhenUsersExist()
    {
        // Arrange
        var users = _faker.Generate(10);
        var query = new GetUsersQuery(1, 5);

        _userRepository.GetUsersAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(users.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.Equal(5, result.Value.Count());
    }

    [Fact]
    public async Task GetUsersQueryHandler_ShouldReturnFailure_WhenNoUsersExist()
    {
        // Arrange
        _userRepository.GetUsersAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Enumerable.Empty<User>()));

        var query = new GetUsersQuery(1, 5);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.User.UsersTableIsEmpty, result.Error);
    }

    [Fact]
    public async Task GetUsersQueryHandler_ShouldReturnFailure_WhenPageIsZero()
    {
        // Arrange
        var users = _faker.Generate(10);
        var query = new GetUsersQuery(0, 5);

        _userRepository.GetUsersAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(users.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Pagination.InvalidPage, result.Error);
    }

    [Fact]
    public async Task GetUsersQueryHandler_ShouldReturnFailure_WhenPageSizeIsZero()
    {
        // Arrange
        var users = _faker.Generate(10);
        var query = new GetUsersQuery(1, 0);

        _userRepository.GetUsersAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(users.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Pagination.InvalidPageSize, result.Error);
    }

    [Fact]
    public async Task GetUsersQueryHandler_ShouldReturnFailure_WhenPageExceedsTotalPages()
    {
        // Arrange
        var users = _faker.Generate(3);
        _userRepository.GetUsersAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(users.AsEnumerable()));

        var query = new GetUsersQuery(5, 3);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Page 5 exceeds the total number of pages (1)", result.Error.Message);
    }

    [Fact]
    public async Task GetUsersQueryHandler_ShouldReturnOrderedUsers_WhenOrderIsProvided()
    {
        // Arrange
        var users = _faker.Generate(5).OrderBy(u => u.Name.FirstName).ToList();
        _userRepository.GetUsersAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(users.AsEnumerable()));

        var query = new GetUsersQuery { Order = "name asc" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(users.Select(u => u.Name.FirstName), result.Value.Select(u => u.Name.FirstName));
    }

    [Fact]
    public async Task GetUsersQueryHandler_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        _userRepository.GetUsersAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IEnumerable<User>>(new Exception("Database error")));

        var query = new GetUsersQuery { Page = 1, PageSize = 5 };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }
}
