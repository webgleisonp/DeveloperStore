using Bogus;
using DeveloperStore.Application.Abstractions;
using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using NSubstitute;

namespace DeveloperStore.Application.Tests.UseCases.Carts;

public class DeleteCartCommandHandlerTests
{
    private readonly ICartsRepository _cartsRepository;
    private readonly IUnityOfWork _unitOfWork;
    private readonly DeleteCartCommandHandler _handler;
    private readonly Faker _faker;

    public DeleteCartCommandHandlerTests()
    {
        _cartsRepository = Substitute.For<ICartsRepository>();
        _unitOfWork = Substitute.For<IUnityOfWork>();
        _handler = new DeleteCartCommandHandler(_cartsRepository, _unitOfWork);
        _faker = new Faker();
    }

    [Fact]
    public async Task DeleteCartCommandHandler_Should_Delete_Cart_When_Cart_Exists()
    {
        // Arrange
        var cartId = _faker.Random.Number();
        var existingCart = new Cart { Id = cartId, UserId = _faker.Random.Number(), Active = true };
        var command = new DeleteCartCommand(cartId);

        _cartsRepository.GetCartByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(existingCart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        await _cartsRepository.Received(1).DeleteCartAsync(existingCart, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCartCommandHandler_Should_Return_Failure_When_Cart_Does_Not_Exist()
    {
        // Arrange
        var cartId = _faker.Random.Number();
        var command = new DeleteCartCommand(cartId);

        _cartsRepository.GetCartByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Cart>(default!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Cart.CartNotFound, result.Error);

        await _cartsRepository.DidNotReceive().DeleteCartAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCartCommandHandler_Should_Call_Repositories_Correctly()
    {
        // Arrange
        var cartId = _faker.Random.Number();
        var existingCart = new Cart { Id = cartId, UserId = _faker.Random.Number(), Active = true };
        var command = new DeleteCartCommand(cartId);

        _cartsRepository.GetCartByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(existingCart);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _cartsRepository.Received(1).GetCartByIdAsync(cartId, Arg.Any<CancellationToken>());
        await _cartsRepository.Received(1).DeleteCartAsync(existingCart, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}