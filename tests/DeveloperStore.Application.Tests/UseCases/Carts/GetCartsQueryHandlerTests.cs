using Bogus;
using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Carts;

public class GetCartsQueryHandlerTests
{
    private readonly ICartsRepository _cartsRepository = Substitute.For<ICartsRepository>();
    private readonly ICartItemsRepository _cartItemsRepository = Substitute.For<ICartItemsRepository>();
    private readonly GetCartsQueryHandler _handler;

    public GetCartsQueryHandlerTests()
    {
        _handler = new GetCartsQueryHandler(_cartsRepository, _cartItemsRepository);
    }

    private static Faker<Cart> CartFaker =>
        new Faker<Cart>()
            .RuleFor(c => c.Id, f => f.Random.Int(1, 1000))
            .RuleFor(c => c.UserId, f => f.Random.Number())
            .RuleFor(c => c.CreateDate, f => f.Date.Past());

    private static Faker<CartItem> CartItemFaker =>
        new Faker<CartItem>()
            .RuleFor(ci => ci.CartId, f => f.Random.Int(1, 1000))
            .RuleFor(ci => ci.ProductId, f => f.Random.Int(1, 500))
            .RuleFor(ci => ci.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(ci => ci.Price, f => f.Random.Decimal(1, 100));

    [Fact]
    public async Task GetCartsQueryHandler_ShouldReturnSuccess_WhenCartsAndItemsExist()
    {
        // Arrange
        var carts = CartFaker.Generate(5);
        var cartItems = CartItemFaker.Generate(10);

        _cartsRepository.GetCartsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<Cart>>(carts));
        _cartItemsRepository.GetCartItemsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<CartItem>>(cartItems));

        var query = new GetCartsQuery { Page = 1, PageSize = 5 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value.Count());
    }

    [Fact]
    public async Task GetCartsQueryHandler_ShouldReturnFailure_WhenNoCartsExist()
    {
        // Arrange
        _cartsRepository.GetCartsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Cart>>(default!));

        var query = new GetCartsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Cart.CartsTableIsEmpty, result.Error);
    }

    [Fact]
    public async Task GetCartsQueryHandler_ShouldReturnFailure_WhenNoCartItemsExist()
    {
        // Arrange
        var carts = CartFaker.Generate(5);
        _cartsRepository.GetCartsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<Cart>>(carts));
        _cartItemsRepository.GetCartItemsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<CartItem>>(default!));

        var query = new GetCartsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.CartItem.CartItemsTableIsEmpty, result.Error);
    }

    [Fact]
    public async Task GetCartsQueryHandler_ShouldApplyPagination_WhenPageAndPageSizeAreProvided()
    {
        // Arrange
        var carts = CartFaker.Generate(10);
        _cartsRepository.GetCartsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<Cart>>(carts));
        _cartItemsRepository.GetCartItemsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<CartItem>>(CartItemFaker.Generate(20)));

        var query = new GetCartsQuery { Page = 1, PageSize = 5 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value.Count());
    }

    [Fact]
    public async Task GetCartsQueryHandler_ShouldReturnFailure_WhenPageIsInvalid()
    {
        // Arrange
        var carts = CartFaker.Generate(10);
        _cartsRepository.GetCartsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<Cart>>(carts));
        _cartItemsRepository.GetCartItemsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<CartItem>>(CartItemFaker.Generate(20)));

        var query = new GetCartsQuery { Page = -1, PageSize = 5 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Pagination.InvalidPage, result.Error);
    }

    [Fact]
    public async Task GetCartsQueryHandler_ShouldOrderResults_WhenOrderParameterIsProvided()
    {
        // Arrange
        var carts = new List<Cart>
        {
            new Cart { Id = 3, UserId = 1, CreateDate = new DateTime(2023, 1, 3) },
            new Cart { Id = 1, UserId = 1, CreateDate = new DateTime(2023, 1, 1) },
            new Cart { Id = 2, UserId = 1, CreateDate = new DateTime(2023, 1, 2) }
        };

        _cartsRepository.GetCartsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<Cart>>(carts));
        _cartItemsRepository.GetCartItemsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IEnumerable<CartItem>>(CartItemFaker.Generate(5)));

        var query = new GetCartsQuery { Order = "id asc" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var orderedIds = result.Value.Select(c => c.Id).ToList();
        Assert.Equal(new List<int> { 1, 2, 3 }, orderedIds);
    }
}
