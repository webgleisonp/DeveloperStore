using Bogus;
using DeveloperStore.Application.Abstractions;
using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Carts;

public class DeleteCartItemCommandHandlerTests
{
    private readonly ICartItemsRepository _cartItemsRepository;
    private readonly IUnityOfWork _unitOfWork;
    private readonly DeleteCartItemCommandHandler _handler;
    private readonly Faker _faker;

    public DeleteCartItemCommandHandlerTests()
    {
        _cartItemsRepository = Substitute.For<ICartItemsRepository>();
        _unitOfWork = Substitute.For<IUnityOfWork>();
        _handler = new DeleteCartItemCommandHandler(_cartItemsRepository, _unitOfWork);
        _faker = new Faker();
    }

    [Fact]
    public async Task DeleteCartItemCommandHandler_Should_Delete_CartItem_When_It_Exists()
    {
        // Arrange
        var cartId = _faker.Random.Number();
        var cartItemId = _faker.Random.Number();
        var existingCartItem = new CartItem { Id = cartItemId, CartId = cartId, ProductId = _faker.Random.Number(), Quantity = 2 };
        var command = new DeleteCartItemCommand(cartId, cartItemId);

        _cartItemsRepository.GetItemByIdAsync(cartItemId, Arg.Any<CancellationToken>()).Returns(existingCartItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        await _cartItemsRepository.Received(1).DeleteCartItemAsync(existingCartItem, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCartItemCommandHandler_Should_Return_Failure_When_CartItem_Does_Not_Exist()
    {
        // Arrange
        var cartId = _faker.Random.Number();
        var cartItemId = _faker.Random.Number();
        var command = new DeleteCartItemCommand(cartId, cartItemId);

        _cartItemsRepository.GetItemByIdAsync(cartItemId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<CartItem>(default!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.CartItem.CartItemNotFound, result.Error);

        await _cartItemsRepository.DidNotReceive().DeleteCartItemAsync(Arg.Any<CartItem>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCartItemCommandHandler_Should_Call_Repositories_Correctly()
    {
        // Arrange
        var cartId = _faker.Random.Number();
        var cartItemId = _faker.Random.Number();
        var existingCartItem = new CartItem { Id = cartItemId, CartId = cartId, ProductId = _faker.Random.Number(), Quantity = 2 };
        var command = new DeleteCartItemCommand(cartId, cartItemId);

        _cartItemsRepository.GetItemByIdAsync(cartItemId, Arg.Any<CancellationToken>()).Returns(existingCartItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _cartItemsRepository.Received(1).GetItemByIdAsync(cartItemId, Arg.Any<CancellationToken>());
        await _cartItemsRepository.Received(1).DeleteCartItemAsync(existingCartItem, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
