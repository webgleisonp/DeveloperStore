using Bogus;
using DeveloperStore.Application.Usecases.Products;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.ValueObjects;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Products;

public class GetProductsQueryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly GetProductsQueryHandler _handler;
    private readonly Faker<Product> _faker;

    public GetProductsQueryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new GetProductsQueryHandler(_productRepository);

        _faker = new Faker<Product>()
            .RuleFor(p => p.Id, f => f.Random.Number())
            .RuleFor(p => p.Title, f => f.Commerce.ProductName())
            .RuleFor(p => p.Price, f => f.Random.Decimal(10, 1000))
            .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
            .RuleFor(p => p.Image, f => f.Image.PicsumUrl())
            .RuleFor(p => p.Rating, f => new Rating(f.Random.Decimal(), f.Random.Number()));
    }

    [Fact]
    public async Task GetProductsQueryHandler_ShouldReturnSuccess_WhenProductsExist()
    {
        // Arrange
        var products = _faker.Generate(5);
        var query = new GetProductsQuery();

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(products));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(products.Count, result.Value.Count());
    }

    [Fact]
    public async Task GetProductsQueryHandler_ShouldReturnFailure_WhenNoProductsExist()
    {
        // Arrange
        var query = new GetProductsQuery();

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Enumerable.Empty<Product>()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Product.ProductsTableIsEmpty, result.Error);
    }

    [Fact]
    public async Task GetProductsQueryHandler_ShouldReturnFailure_WhenRepositoryReturnsNull()
    {
        // Arrange
        var query = new GetProductsQuery();

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(default!));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Product.ProductsTableIsEmpty, result.Error);
    }

    [Fact]
    public async Task GetProductsQueryHandler_ShouldOrderProductsByPriceAsc()
    {
        // Arrange
        var products = _faker.Generate(5);
        var query = new GetProductsQuery(1, 5, "price asc");

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(products));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(products.OrderBy(p => p.Price).Select(p => p.Title), result.Value.Select(r => r.Title));
    }

    [Fact]
    public async Task GetProductsQueryHandler_ShouldOrderProductsByPriceDesc()
    {
        // Arrange
        var products = _faker.Generate(5);
        var query = new GetProductsQuery(1, 5, "price desc");

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(products));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(products.OrderByDescending(p => p.Price).Select(p => p.Title), result.Value.Select(r => r.Title));
    }

    [Fact]
    public async Task GetProductsQueryHandler_ShouldReturnFailure_WhenPageSizeIsZero()
    {
        // Arrange
        var products = _faker.Generate(5);
        var query = new GetProductsQuery(1, 0);

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(products));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Pagination.InvalidPageSize, result.Error);
    }

    [Fact]
    public async Task GetProductsQueryHandler_ShouldReturnFailure_WhenPageExceedsLimit()
    {
        // Arrange
        var products = _faker.Generate(5);
        var query = new GetProductsQuery(10, 2);

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(products));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Pagination.PageExceedsLimit(10, 3), result.Error);
    }
}