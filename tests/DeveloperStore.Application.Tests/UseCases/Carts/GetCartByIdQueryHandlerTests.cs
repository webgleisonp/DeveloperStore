using Bogus;
using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Carts;

public class GetCartByIdQueryHandlerTests
{
    private readonly ICartsRepository _cartsRepository;
    private readonly ICartItemsRepository _cartItemsRepository;
    private readonly GetCartByIdQueryHandler _handler;
    private readonly Faker _faker;

    public GetCartByIdQueryHandlerTests()
    {
        _cartsRepository = Substitute.For<ICartsRepository>();
        _cartItemsRepository = Substitute.For<ICartItemsRepository>();
        _handler = new GetCartByIdQueryHandler(_cartsRepository, _cartItemsRepository);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetCartByIdQueryHandler_Should_Return_Cart_When_It_Exists()
    {
        // Arrange
        var cartId = _faker.Random.Number();
        var userId = _faker.Random.Number();
        var createDate = DateTime.Now;

        var cart = new Cart { Id = cartId, UserId = userId, CreateDate = createDate, Active = true };
        var cartItems = new List<CartItem>
        {
            new CartItem { Id = _faker.Random.Number(), CartId = cartId, ProductId = _faker.Random.Number(), Quantity = 2, Price = _faker.Random.Decimal(10, 100) },
            new CartItem { Id = _faker.Random.Number(), CartId = cartId, ProductId = _faker.Random.Number(), Quantity = 1, Price = _faker.Random.Decimal(10, 100) }
        };

        var query = new GetCartByIdQuery(cartId);

        _cartsRepository.GetCartByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(cart);
        _cartItemsRepository.GetItemsByCartIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(cartItems);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(cartId, result.Value.Id);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal(cartItems.Count, result.Value.CartItems.Count());

        await _cartsRepository.Received(1).GetCartByIdAsync(cartId, Arg.Any<CancellationToken>());
        await _cartItemsRepository.Received(1).GetItemsByCartIdAsync(cartId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCartByIdQueryHandler_Should_Return_Failure_When_Cart_Does_Not_Exist()
    {
        // Arrange
        var cartId = _faker.Random.Number();
        var query = new GetCartByIdQuery(cartId);

        _cartsRepository.GetCartByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Cart>(default!));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Cart.CartNotFound, result.Error);

        await _cartsRepository.Received(1).GetCartByIdAsync(cartId, Arg.Any<CancellationToken>());
        await _cartItemsRepository.DidNotReceive().GetItemsByCartIdAsync(cartId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCartByIdQueryHandler_Should_Call_Repositories_Correctly()
    {
        // Arrange
        var cartId = _faker.Random.Number();
        var cart = new Cart { Id = cartId, UserId = _faker.Random.Number(), CreateDate = _faker.Date.Past(), Active = true };
        var cartItems = new List<CartItem>();

        var query = new GetCartByIdQuery(cartId);

        _cartsRepository.GetCartByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(cart);
        _cartItemsRepository.GetItemsByCartIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(cartItems);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _cartsRepository.Received(1).GetCartByIdAsync(cartId, Arg.Any<CancellationToken>());
        await _cartItemsRepository.Received(1).GetItemsByCartIdAsync(cartId, Arg.Any<CancellationToken>());
    }
}
