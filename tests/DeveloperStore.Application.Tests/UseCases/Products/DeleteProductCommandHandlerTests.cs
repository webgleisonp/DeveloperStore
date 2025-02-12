using Bogus;
using DeveloperStore.Application.Abstractions;
using DeveloperStore.Application.Usecases.Products;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.ValueObjects;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Products;

public class DeleteProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IUnityOfWork _unitOfWork;
    private readonly DeleteProductCommandHandler _handler;
    private readonly Faker<Product> _faker;

    public DeleteProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _unitOfWork = Substitute.For<IUnityOfWork>();
        _handler = new DeleteProductCommandHandler(_productRepository, _unitOfWork);

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
    public async Task DeleteProductCommandHandler_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        var existingProduct = _faker.Generate();
        var command = new DeleteProductCommand(existingProduct.Id);

        _productRepository.GetProductByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(existingProduct));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        await _productRepository.Received(1).DeleteProductAsync(existingProduct, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteProductCommandHandler_ShouldReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new DeleteProductCommand(1);

        _productRepository.GetProductByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product>(default!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Product.ProductNotFound, result.Error);

        await _productRepository.DidNotReceive().DeleteProductAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteProductCommandHandler_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var existingProduct = _faker.Generate();
        var command = new DeleteProductCommand(existingProduct.Id);

        _productRepository.GetProductByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(existingProduct));

        _productRepository.DeleteProductAsync(existingProduct, Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
