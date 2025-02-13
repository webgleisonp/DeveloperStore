using Bogus;
using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using FluentValidation.TestHelper;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Carts;

public class DeleteCartItemCommandValidatorTests
{
    private readonly ICartsRepository _cartsRepository;
    private readonly ICartItemsRepository _cartItemsRepository;
    private readonly DeleteCartItemCommandValidator _validator;
    private readonly Faker _faker;

    public DeleteCartItemCommandValidatorTests()
    {
        _cartsRepository = Substitute.For<ICartsRepository>();
        _cartItemsRepository = Substitute.For<ICartItemsRepository>();
        _validator = new DeleteCartItemCommandValidator(_cartsRepository, _cartItemsRepository);
        _faker = new Faker();
    }

    [Fact]
    public async Task DeleteCartItemCommandValidator_Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var cartId = _faker.Random.Number(1, 100);
        var cartItemId = _faker.Random.Number(1, 100);
        var existingCart = new Cart
        {
            Id = cartId,
            UserId = _faker.Random.Number(1, 100),
            CreateDate = _faker.Date.Recent(),
            Active = true
        };

        var existingCartItem = new CartItem
        {
            Id = cartItemId,
            CartId = cartId,
            ProductId = _faker.Random.Number(),
            Quantity = _faker.Random.Number(1, 10)
        };

        _cartsRepository.GetCartByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(existingCart);

        _cartItemsRepository.GetItemByIdAsync(cartItemId, Arg.Any<CancellationToken>())
            .Returns(existingCartItem);

        var command = new DeleteCartItemCommand(cartId, cartItemId);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task DeleteCartItemCommandValidator_Should_Fail_When_CartId_Is_Empty()
    {
        // Arrange
        var command = new DeleteCartItemCommand(0, _faker.Random.Number(1, 100));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CartId);
    }

    [Fact]
    public async Task DeleteCartItemCommandValidator_Should_Fail_When_CartItemId_Is_Empty()
    {
        // Arrange
        var command = new DeleteCartItemCommand(_faker.Random.Number(1, 100), 0);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CartItemId);
    }

    [Fact]
    public async Task DeleteCartItemCommandValidator_Should_Fail_When_CartItem_Does_Not_Exist()
    {
        // Arrange
        var cartId = _faker.Random.Number(1, 100);
        var cartItemId = _faker.Random.Number(1, 100);

        _cartItemsRepository.GetItemByIdAsync(cartItemId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<CartItem>(null!));

        var command = new DeleteCartItemCommand(cartId, cartItemId);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CartItemId)
              .WithErrorCode(DomainErrors.CartItem.CartItemNotFound.Code);
    }
}