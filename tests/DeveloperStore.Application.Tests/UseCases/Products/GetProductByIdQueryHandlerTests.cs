using Bogus;
using DeveloperStore.Application.Usecases.Products;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.ValueObjects;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Products;

public class GetProductByIdQueryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly GetProductByIdQueryHandler _handler;
    private readonly Faker<Product> _faker;

    public GetProductByIdQueryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new GetProductByIdQueryHandler(_productRepository);

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
    public async Task GetProductByIdQueryHandler_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var existingProduct = _faker.Generate();
        var query = new GetProductByIdQuery(existingProduct.Id);

        _productRepository.GetProductByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(existingProduct));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(existingProduct.Id, result.Value.Id);
        Assert.Equal(existingProduct.Title, result.Value.Title);

        await _productRepository.Received(1).GetProductByIdAsync(query.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductByIdQueryHandler_ShouldReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var query = new GetProductByIdQuery(1);

        _productRepository.GetProductByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product>(default!));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Product.ProductNotFound, result.Error);

        await _productRepository.Received(1).GetProductByIdAsync(query.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductByIdQueryHandler_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var query = new GetProductByIdQuery(1);

        _productRepository.GetProductByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Product>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }
}
