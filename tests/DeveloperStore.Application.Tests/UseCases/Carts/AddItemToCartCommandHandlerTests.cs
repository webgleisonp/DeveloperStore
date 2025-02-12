using Bogus;
using DeveloperStore.Application.Abstractions;
using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Carts;

public class AddItemToCartCommandHandlerTests
{
    private readonly ICartsRepository _cartRepository;
    private readonly ICartItemsRepository _cartItemsRepository;
    private readonly IUnityOfWork _unityOfWork;
    private readonly AddItemToCartCommandHandler _handler;
    private readonly Faker<CartItem> _cartItemFaker;
    private readonly Faker<Cart> _cartFaker;

    public AddItemToCartCommandHandlerTests()
    {
        _cartRepository = Substitute.For<ICartsRepository>();
        _cartItemsRepository = Substitute.For<ICartItemsRepository>();
        _unityOfWork = Substitute.For<IUnityOfWork>();
        _handler = new AddItemToCartCommandHandler(_cartRepository, _cartItemsRepository, _unityOfWork);

        _cartItemFaker = new Faker<CartItem>()
            .RuleFor(ci => ci.CartId, f => f.Random.Number())
            .RuleFor(ci => ci.ProductId, f => f.Random.Number())
            .RuleFor(ci => ci.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(ci => ci.Price, f => f.Random.Decimal(10, 1000));

        _cartFaker = new Faker<Cart>()
            .RuleFor(c => c.Id, f => f.Random.Number())
            .RuleFor(c => c.UserId, f => f.Random.Number())
            .RuleFor(c => c.CreateDate, f => f.Date.Past());
    }

    [Fact]
    public async Task AddItemToCartCommandHandler_ShouldReturnSuccess_WhenItemIsAddedToCart()
    {
        // Arrange
        var cart = _cartFaker.Generate();
        var cartItems = _cartItemFaker.Generate(1);
        var command = new AddItemToCartCommand(cart.Id, 1, 2, 50m);

        _cartItemsRepository.GetCartItemByProductIdAsync(command.CartId, command.ProductId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<CartItem>(default!));

        _cartRepository.GetCartByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(cart));

        _cartItemsRepository.GetItemsByCartIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(cartItems.AsEnumerable()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(cart.Id, result.Value.Id);
        Assert.Equal(cart.UserId, result.Value.UserId);
        Assert.Single(result.Value.CartItems);
    }

    [Fact]
    public async Task AddItemToCartCommandHandler_ShouldReturnFailure_WhenItemAlreadyExistsInCart()
    {
        // Arrange
        var cartItem = _cartItemFaker.Generate();
        var command = new AddItemToCartCommand(cartItem.CartId, cartItem.ProductId, 2, 50m);

        _cartItemsRepository.GetCartItemByProductIdAsync(command.CartId, command.ProductId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(cartItem));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.CartItem.CartItemExists, result.Error);
    }

    [Fact]
    public async Task AddItemToCartCommandHandler_ShouldReturnFailure_WhenErrorOccursWhileSavingChanges()
    {
        // Arrange
        var cart = _cartFaker.Generate();
        var command = new AddItemToCartCommand(cart.Id, 1, 2, 50m);

        _cartItemsRepository.GetCartItemByProductIdAsync(command.CartId, command.ProductId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CartItem>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task AddItemToCartCommandHandler_ShouldReturnFailure_WhenCartNotFoundAfterAddingItem()
    {
        // Arrange
        var command = new AddItemToCartCommand(1, 1, 2, 50m);

        _cartItemsRepository.GetCartItemByProductIdAsync(command.CartId, command.ProductId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<CartItem>(default!));

        _cartRepository.GetCartByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Cart>(default!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Cart.CartNotFound, result.Error);
    }
}