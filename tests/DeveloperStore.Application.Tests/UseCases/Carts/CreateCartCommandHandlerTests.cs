using Bogus;
using DeveloperStore.Application.Abstractions;
using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Carts;

public class CreateCartCommandHandlerTests
{
    private readonly ICartsRepository _cartsRepository;
    private readonly ICartItemsRepository _cartItemsRepository;
    private readonly IUnityOfWork _unitOfWork;
    private readonly CreateCartCommandHandler _handler;
    private readonly Faker _faker;

    public CreateCartCommandHandlerTests()
    {
        _cartsRepository = Substitute.For<ICartsRepository>();
        _cartItemsRepository = Substitute.For<ICartItemsRepository>();
        _unitOfWork = Substitute.For<IUnityOfWork>();
        _handler = new CreateCartCommandHandler(_cartsRepository, _cartItemsRepository, _unitOfWork);
        _faker = new Faker();
    }

    [Fact]
    public async Task CreateCartCommandHandler_Should_Create_Cart_When_No_Existing_Cart()
    {
        // Arrange
        var userId = _faker.Random.Number();
        var cartItems = new List<CartItemsRequest>
        {
            new CartItemsRequest(_faker.Random.Number(), _faker.Random.Int(1, 5), _faker.Random.Decimal(10, 100))
        };

        var command = new CreateCartCommand(userId, DateTime.UtcNow, cartItems);

        _cartsRepository.GetCartByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Cart>(default!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.UserId);

        await _cartItemsRepository.Received(cartItems.Count).CreateCartItemAsync(Arg.Any<CartItem>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCartCommandHandler_Should_Return_Failure_When_Cart_Already_Exists()
    {
        // Arrange
        var userId = _faker.Random.Number();
        var existingCart = new Cart { UserId = userId, CreateDate = DateTime.UtcNow, Active = true };
        var command = new CreateCartCommand(userId, DateTime.UtcNow, new List<CartItemsRequest>());

        _cartsRepository.GetCartByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingCart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Cart.CartExists, result.Error);

        await _cartItemsRepository.DidNotReceive().CreateCartItemAsync(Arg.Any<CartItem>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCartCommandHandler_Should_Create_Cart_With_Correct_Values()
    {
        // Arrange
        var userId = _faker.Random.Number();
        var createDate = DateTime.UtcNow;
        var cartItems = new List<CartItemsRequest>
        {
            new CartItemsRequest(_faker.Random.Number(), _faker.Random.Int(1, 5), _faker.Random.Decimal(10, 100)),
            new CartItemsRequest(_faker.Random.Number(), _faker.Random.Int(1, 5), _faker.Random.Decimal(10, 100))
        };

        var command = new CreateCartCommand(userId, createDate, cartItems);

        _cartsRepository.GetCartByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Cart>(default!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal(createDate, result.Value.CreateDate);
        Assert.Equal(cartItems.Count, result.Value.CartItems.Count());

        await _cartItemsRepository.Received(cartItems.Count).CreateCartItemAsync(Arg.Any<CartItem>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}