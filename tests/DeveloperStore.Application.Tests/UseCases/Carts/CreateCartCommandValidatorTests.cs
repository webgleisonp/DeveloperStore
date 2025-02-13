using Bogus;
using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.ValueObjects;
using FluentValidation.TestHelper;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Carts;

public class CreateCartCommandValidatorTests
{
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly CreateCartCommandValidator _validator;
    private readonly Faker _faker;

    public CreateCartCommandValidatorTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _validator = new CreateCartCommandValidator(_userRepository, _productRepository);
        _faker = new Faker();
    }

    [Fact]
    public async Task CreateCartCommandValidator_Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var userId = _faker.Random.Number();
        var produtcId = _faker.Random.Number();
        var cartItems = new List<CartItemsRequest>
        {
            new(produtcId, _faker.Random.Number(1, 10), _faker.Random.Decimal(1, 1000))
        };

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new User
            {
                Id = userId,
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password(),
                Name = new(_faker.Name.FirstName(), _faker.Name.LastName()),
                Address = new(
                    _faker.Address.City(),
                    _faker.Address.StreetName(),
                    _faker.Random.Number(1, 1000),
                    _faker.Address.ZipCode(),
                    _faker.Address.Latitude().ToString("F2"),
                    _faker.Address.Longitude().ToString("F2")
                ),
                Phone = _faker.Phone.PhoneNumber(),
                Status = Status.Active,
                Role = Role.Admin
            });

        _productRepository.GetProductByIdAsync(produtcId, Arg.Any<CancellationToken>())
            .Returns(new Product {
                Id = produtcId,
                Title = _faker.Commerce.ProductName(),
                Description = _faker.Commerce.ProductDescription(),
                Category = _faker.Commerce.Department(),
                Image = _faker.Image.PlaceImgUrl(),
                Price = _faker.Random.Decimal(),
                Rating = new Rating(_faker.Random.Decimal(), _faker.Random.Number())
            });

        var command = new CreateCartCommand(userId, DateTime.UtcNow, cartItems);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateCartCommandValidator_Should_Fail_When_UserId_Is_Empty()
    {
        // Arrange
        var cartItems = new List<CartItemsRequest>
        {
            new(_faker.Random.Number(1, 100), _faker.Random.Number(1, 10), _faker.Random.Decimal(1, 1000))
        };

        var command = new CreateCartCommand(0, DateTime.UtcNow, cartItems);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UserId);
    }

    [Fact]
    public async Task CreateCartCommandValidator_Should_Fail_When_UserId_Does_Not_Exist()
    {
        // Arrange
        var userId = _faker.Random.Number(1, 100);
        var cartItems = new List<CartItemsRequest>
        {
            new(_faker.Random.Number(1, 100), _faker.Random.Number(1, 10), _faker.Random.Decimal(1, 1000))
        };

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User>(null!));

        var command = new CreateCartCommand(userId, DateTime.UtcNow, cartItems);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UserId)
              .WithErrorCode(DomainErrors.User.UserNotFound.Code);
    }

    [Fact]
    public async Task CreateCartCommandValidator_Should_Fail_When_CreateDate_Is_Empty()
    {
        // Arrange
        var userId = _faker.Random.Number();
        var produtcId = _faker.Random.Number();
        var cartItems = new List<CartItemsRequest>
        {
            new(_faker.Random.Number(1, 100), _faker.Random.Number(1, 10), _faker.Random.Decimal(1, 1000))
        };

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new User
            {
                Id = userId,
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password(),
                Name = new(_faker.Name.FirstName(), _faker.Name.LastName()),
                Address = new(
                    _faker.Address.City(),
                    _faker.Address.StreetName(),
                    _faker.Random.Number(1, 1000),
                    _faker.Address.ZipCode(),
                    _faker.Address.Latitude().ToString("F2"),
                    _faker.Address.Longitude().ToString("F2")
                ),
                Phone = _faker.Phone.PhoneNumber(),
                Status = Status.Active,
                Role = Role.Admin
            });

        _productRepository.GetProductByIdAsync(produtcId, Arg.Any<CancellationToken>())
            .Returns(new Product
            {
                Id = produtcId,
                Title = _faker.Commerce.ProductName(),
                Description = _faker.Commerce.ProductDescription(),
                Category = _faker.Commerce.Department(),
                Image = _faker.Image.PlaceImgUrl(),
                Price = _faker.Random.Decimal(),
                Rating = new Rating(_faker.Random.Decimal(), _faker.Random.Number())
            });

        var command = new CreateCartCommand(userId, default, cartItems);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateDate);
    }

    [Fact]
    public async Task CreateCartCommandValidator_Should_Fail_When_CartItems_Is_Empty()
    {
        // Arrange
        var userId = _faker.Random.Number();
        var produtcId = _faker.Random.Number();

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new User
            {
                Id = userId,
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password(),
                Name = new(_faker.Name.FirstName(), _faker.Name.LastName()),
                Address = new(
                    _faker.Address.City(),
                    _faker.Address.StreetName(),
                    _faker.Random.Number(1, 1000),
                    _faker.Address.ZipCode(),
                    _faker.Address.Latitude().ToString("F2"),
                    _faker.Address.Longitude().ToString("F2")
                ),
                Phone = _faker.Phone.PhoneNumber(),
                Status = Status.Active,
                Role = Role.Admin
            });

        _productRepository.GetProductByIdAsync(produtcId, Arg.Any<CancellationToken>())
            .Returns(new Product
            {
                Id = produtcId,
                Title = _faker.Commerce.ProductName(),
                Description = _faker.Commerce.ProductDescription(),
                Category = _faker.Commerce.Department(),
                Image = _faker.Image.PlaceImgUrl(),
                Price = _faker.Random.Decimal(),
                Rating = new Rating(_faker.Random.Decimal(), _faker.Random.Number())
            });

        var command = new CreateCartCommand(userId, DateTime.UtcNow, new List<CartItemsRequest>());

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CartItems);
    }

    [Fact]
    public async Task CreateCartCommandValidator_Should_Fail_When_CartItems_Contains_Invalid_Items()
    {
        // Arrange
        var userId = _faker.Random.Number();
        var produtcId = _faker.Random.Number();
        var cartItems = new List<CartItemsRequest>
        {
            new(0, 0, 0) // Item inválido
        };

        _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new User
            {
                Id = userId,
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password(),
                Name = new(_faker.Name.FirstName(), _faker.Name.LastName()),
                Address = new(
                    _faker.Address.City(),
                    _faker.Address.StreetName(),
                    _faker.Random.Number(1, 1000),
                    _faker.Address.ZipCode(),
                    _faker.Address.Latitude().ToString("F2"),
                    _faker.Address.Longitude().ToString("F2")
                ),
                Phone = _faker.Phone.PhoneNumber(),
                Status = Status.Active,
                Role = Role.Admin
            });

        _productRepository.GetProductByIdAsync(produtcId, Arg.Any<CancellationToken>())
            .Returns(new Product
            {
                Id = produtcId,
                Title = _faker.Commerce.ProductName(),
                Description = _faker.Commerce.ProductDescription(),
                Category = _faker.Commerce.Department(),
                Image = _faker.Image.PlaceImgUrl(),
                Price = _faker.Random.Decimal(),
                Rating = new Rating(_faker.Random.Decimal(), _faker.Random.Number())
            });

        var command = new CreateCartCommand(userId, DateTime.UtcNow, cartItems);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveAnyValidationError();
    }
}