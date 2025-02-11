using Bogus;
using DeveloperStore.Application.Abstractions;
using DeveloperStore.Application.Usecases.Products;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.ValueObjects;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Products;

public class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IUnityOfWork _unitOfWork;
    private readonly CreateProductCommandHandler _handler;
    private readonly Faker<Product> _faker;

    public CreateProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _unitOfWork = Substitute.For<IUnityOfWork>();
        _handler = new CreateProductCommandHandler(_productRepository, _unitOfWork);

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
    public async Task Handle_ShouldCreateProduct_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new CreateProductCommand("New Product",
            100.50m,
            "Product description",
            "Electronics",
            "https://example.com/image.jpg",
            new Rating(4.5m, 10)
        );

        _productRepository.GetProductsByTitleAsync(command.Title, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Title, result.Value.Title);
        Assert.Equal(command.Price, result.Value.Price);
        Assert.Equal(command.Description, result.Value.Description);
        Assert.Equal(command.Category, result.Value.Category);
        Assert.Equal(command.Image, result.Value.Image);
        Assert.Equal(command.Rating, result.Value.Rating);

        await _productRepository.Received(1).CreateProductsAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductAlreadyExists()
    {
        // Arrange
        var existingProduct = _faker.Generate();
        var command = new CreateProductCommand(existingProduct.Title, 100, "test","test","teste.jpg", new Rating(1,1));

        _productRepository.GetProductsByTitleAsync(command.Title, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(existingProduct));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Product.ProductExists, result.Error);

        await _productRepository.DidNotReceive().CreateProductsAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var command = new CreateProductCommand("test", 100, "test", "test", "teste.jpg", new Rating(1, 1));

        _productRepository.GetProductsByTitleAsync(command.Title, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Product>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
