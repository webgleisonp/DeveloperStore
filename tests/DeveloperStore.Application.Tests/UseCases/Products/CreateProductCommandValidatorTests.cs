using Bogus;
using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using FluentValidation.TestHelper;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Products;

public class CreateCartCommandValidatorTests
{
    private readonly CreateCartCommandValidator _validator;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly Faker _faker;

    public CreateCartCommandValidatorTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _productRepository = Substitute.For<IProductRepository>();

        _validator = new CreateCartCommandValidator(_userRepository, _productRepository);
        _faker = new Faker();
    }

    [Fact]
    public async Task Validate_Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var validUserId = _faker.Random.Number();
        var validProductId = _faker.Random.Number();
        _userRepository.GetUserByIdAsync(validUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new User()));

        _productRepository.GetProductByIdAsync(validProductId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new Product()));

        var command = new CreateCartCommand(validUserId,
            DateTime.UtcNow,
            new List<CartItemsRequest>
            {
                new CartItemsRequest(validProductId, 2, 50)
            }
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_Should_Fail_When_UserId_Is_Invalid()
    {
        // Arrange
        var invalidUserId = _faker.Random.Number();
        _userRepository.GetUserByIdAsync(invalidUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User>(default!)); // Simula que o usuário não existe

        var command = new CreateCartCommand(invalidUserId,
            DateTime.UtcNow,
            new List<CartItemsRequest>
            {
                new CartItemsRequest(_faker.Random.Number(), 2, 50)
            }
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UserId)
              .WithErrorCode(DomainErrors.User.UserNotFound.Code);
    }

    [Fact]
    public async Task Validate_Should_Fail_When_CreateDate_Is_Empty()
    {
        // Arrange
        var command = new CreateCartCommand(_faker.Random.Number(),
            default,
            new List<CartItemsRequest>
            {
                new CartItemsRequest(_faker.Random.Number(), 2, 50)
            }
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateDate);
    }

    [Fact]
    public async Task Validate_Should_Fail_When_CartItems_Is_Empty()
    {
        // Arrange
        var command = new CreateCartCommand(_faker.Random.Number(),
            default,
            new List<CartItemsRequest>());

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CartItems);
    }

    [Fact]
    public async Task Validate_Should_Fail_When_Any_CartItem_Is_Null()
    {
        // Arrange
        var command = new CreateCartCommand(_faker.Random.Number(),
            DateTime.UtcNow,
            new List<CartItemsRequest> { null });

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor("CartItems[0]");
    }
}

