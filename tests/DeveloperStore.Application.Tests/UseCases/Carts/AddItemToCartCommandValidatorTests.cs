using Bogus;
using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using FluentValidation.TestHelper;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Carts;

public class AddItemToCartCommandValidatorTests
{
    private readonly ICartsRepository _cartsRepository;
    private readonly IProductRepository _productRepository;
    private readonly AddItemToCartCommandValidator _validator;
    private readonly Faker _faker;

    public AddItemToCartCommandValidatorTests()
    {
        _cartsRepository = Substitute.For<ICartsRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _validator = new AddItemToCartCommandValidator(_cartsRepository, _productRepository);
        _faker = new Faker();
    }

    [Fact]
    public async Task AddItemToCartCommandValidator_Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var cartId = _faker.Random.Number();
        var productId = _faker.Random.Number();

        _cartsRepository.GetCartByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(new Cart());
        _productRepository.GetProductByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(new Product());

        var command = new AddItemToCartCommand(cartId, productId, 5, 50.00m);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task AddItemToCartCommandValidator_Should_Fail_When_CartId_Is_Empty()
    {
        // Arrange
        var command = new AddItemToCartCommand(0, _faker.Random.Number(), 5, 50.00m);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CartId);
    }

    [Fact]
    public async Task AddItemToCartCommandValidator_Should_Fail_When_CartId_Does_Not_Exist()
    {
        // Arrange
        var cartId = _faker.Random.Number();
        _cartsRepository.GetCartByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Cart>(default!));

        var command = new AddItemToCartCommand(cartId, _faker.Random.Number(), 5, 50.00m);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CartId)
              .WithErrorCode(DomainErrors.Cart.CartNotFound.Code);
    }

    [Fact]
    public async Task AddItemToCartCommandValidator_Should_Fail_When_ProductId_Is_Empty()
    {
        // Arrange
        var command = new AddItemToCartCommand(_faker.Random.Number(), 0, 5, 50.00m);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ProductId);
    }

    [Fact]
    public async Task AddItemToCartCommandValidator_Should_Fail_When_ProductId_Does_Not_Exist()
    {
        // Arrange
        var productId = _faker.Random.Number();
        _productRepository.GetProductByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product>(default!));

        var command = new AddItemToCartCommand(_faker.Random.Number(), productId, 5, 50.00m);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ProductId)
              .WithErrorCode(DomainErrors.Product.ProductNotFound.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task AddItemToCartCommandValidator_Should_Fail_When_Quantity_Is_Zero_Or_Less(int invalidQuantity)
    {
        // Arrange
        var command = new AddItemToCartCommand(_faker.Random.Number(), _faker.Random.Number(), invalidQuantity, 50.00m);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Quantity);
    }

    [Fact]
    public async Task AddItemToCartCommandValidator_Should_Fail_When_Quantity_Is_Greater_Than_20()
    {
        // Arrange
        var command = new AddItemToCartCommand(_faker.Random.Number(), _faker.Random.Number(), 21, 50.00m);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task AddItemToCartCommandValidator_Should_Fail_When_ItemPrice_Is_Zero_Or_Less(decimal invalidPrice)
    {
        // Arrange
        var command = new AddItemToCartCommand(_faker.Random.Number(), _faker.Random.Number(), 5, invalidPrice);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ItemPrice);
    }
}
