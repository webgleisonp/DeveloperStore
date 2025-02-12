using Bogus;
using DeveloperStore.Application.Abstractions;
using DeveloperStore.Application.Usecases.Products;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.ValueObjects;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Products;

public class UpdateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IUnityOfWork _unityOfWork;
    private readonly UpdateProductCommandHandler _handler;
    private readonly Faker<Product> _faker;

    public UpdateProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _unityOfWork = Substitute.For<IUnityOfWork>();
        _handler = new UpdateProductCommandHandler(_productRepository, _unityOfWork);

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
    public async Task UpdateProductCommandHandler_ShouldReturnSuccess_WhenProductIsFoundAndUpdated()
    {
        // Arrange
        var product = _faker.Generate();
        var command = new UpdateProductCommand(
            product.Id,
            "Updated Product",
            150.00m,
            "Updated Description",
            "Updated Category",
            "updated-image-url",
            new Rating(4.5m,100)
        );

        _productRepository.GetProductByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(product));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Title, result.Value.Title);
        Assert.Equal(command.Price, result.Value.Price);
        Assert.Equal(command.Description, result.Value.Description);
        Assert.Equal(command.Category, result.Value.Category);
        Assert.Equal(command.Image, result.Value.Image);
        Assert.Equal(command.Rating.Rate, result.Value.Rating.Rate);
    }

    [Fact]
    public async Task UpdateProductCommandHandler_ShouldReturnFailure_WhenProductIsNotFound()
    {
        // Arrange
        var command = new UpdateProductCommand(
            1,
            "Updated Product",
            150.00m,
            "Updated Description",
            "Updated Category",
            "updated-image-url",
            new Rating(4.5m, 100)
        );

        _productRepository.GetProductByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product>(default!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Product.ProductNotFound, result.Error);
    }

    [Fact]
    public async Task UpdateProductCommandHandler_ShouldUpdateProductFieldsCorrectly()
    {
        // Arrange
        var product = _faker.Generate();
        var command = new UpdateProductCommand(
            product.Id,
            "Updated Product",
            150.00m,
            "Updated Description",
            "Updated Category",
            "updated-image-url",
            new Rating(4.5m, 100)
        );

        _productRepository.GetProductByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(product));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.Title, product.Title);
        Assert.Equal(command.Price, product.Price);
        Assert.Equal(command.Description, product.Description);
        Assert.Equal(command.Category, product.Category);
        Assert.Equal(command.Image, product.Image);
        Assert.Equal(command.Rating.Rate, product.Rating.Rate);
    }

    [Fact]
    public async Task UpdateProductCommandHandler_ShouldReturnFailure_WhenRepositoryFails()
    {
        // Arrange
        var product = _faker.Generate();
        var command = new UpdateProductCommand(
            product.Id,
            "Updated Product",
            150.00m,
            "Updated Description",
            "Updated Category",
            "updated-image-url",
            new Rating(4.5m, 100)
        );

        _productRepository.GetProductByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Product>(new Exception("Database error")));

        // Act e Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}