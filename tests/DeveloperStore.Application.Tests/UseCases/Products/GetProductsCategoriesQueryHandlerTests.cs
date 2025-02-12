using Bogus;
using DeveloperStore.Application.Usecases.Products;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.ValueObjects;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Products;

public class GetProductsCategoriesQueryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly GetProductsCategoriesQueryHandler _handler;
    private readonly Faker<Product> _faker;

    public GetProductsCategoriesQueryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new GetProductsCategoriesQueryHandler(_productRepository);

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
    public async Task GetProductsCategoriesQueryHandler_ShouldReturnCategories_WhenProductsExist()
    {
        // Arrange
        var products = _faker.Generate(5);
        var distinctCategories = products.Select(p => p.Category).Distinct();
        var query = new GetProductsCategoriesQuery();

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(products));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(distinctCategories.Count(), result.Value.Count());

        await _productRepository.Received(1).GetProductsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductsCategoriesQueryHandler_ShouldReturnFailure_WhenNoProductsExist()
    {
        // Arrange
        var query = new GetProductsCategoriesQuery();

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(new List<Product>()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Product.ProductsTableIsEmpty, result.Error);

        await _productRepository.Received(1).GetProductsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductsCategoriesQueryHandler_ShouldReturnFailure_WhenRepositoryReturnsNull()
    {
        // Arrange
        var query = new GetProductsCategoriesQuery();

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(default!));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Product.ProductsTableIsEmpty, result.Error);

        await _productRepository.Received(1).GetProductsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductsCategoriesQueryHandler_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var query = new GetProductsCategoriesQuery();

        _productRepository.GetProductsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IEnumerable<Product>>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }
}