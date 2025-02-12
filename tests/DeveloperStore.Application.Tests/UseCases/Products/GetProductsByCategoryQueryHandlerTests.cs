using Bogus;
using DeveloperStore.Application.Usecases.Products;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.ValueObjects;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Products;

public class GetProductsByCategoryQueryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly GetProductsByCategoryQueryHandler _handler;
    private readonly Faker<Product> _faker;

    public GetProductsByCategoryQueryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new GetProductsByCategoryQueryHandler(_productRepository);

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
    public async Task GetProductsByCategoryQueryHandler_ShouldReturnProducts_WhenCategoryExists()
    {
        // Arrange
        var category = "Electronics";
        var products = _faker.Generate(3).Select(p => { p.Category = category; return p; });
        var query = new GetProductsByCategoryQuery(category);

        _productRepository.GetProductsByCategoryAsync(category, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(products));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count());

        await _productRepository.Received(1).GetProductsByCategoryAsync(category, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductsByCategoryQueryHandler_ShouldReturnFailure_WhenNoProductsFound()
    {
        // Arrange
        var category = "NonExistentCategory";
        var query = new GetProductsByCategoryQuery(category);

        _productRepository.GetProductsByCategoryAsync(category, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(new List<Product>()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Product.ProductNotFound, result.Error);

        await _productRepository.Received(1).GetProductsByCategoryAsync(category, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductsByCategoryQueryHandler_ShouldReturnFailure_WhenRepositoryReturnsNull()
    {
        // Arrange
        var category = "SomeCategory";
        var query = new GetProductsByCategoryQuery(category);

        _productRepository.GetProductsByCategoryAsync(category, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Product>>(default!));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Product.ProductNotFound, result.Error);

        await _productRepository.Received(1).GetProductsByCategoryAsync(category, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductsByCategoryQueryHandler_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var category = "SomeCategory";
        var query = new GetProductsByCategoryQuery(category);

        _productRepository.GetProductsByCategoryAsync(category, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IEnumerable<Product>>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }
}