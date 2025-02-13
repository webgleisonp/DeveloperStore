using Bogus;
using DeveloperStore.Application.Usecases.Carts;
using DeveloperStore.Domain.Entities;
using FluentValidation.TestHelper;

namespace DeveloperStore.Application.Tests.UseCases.Carts;

public class DeleteCartCommandValidatorTests
{
    private readonly DeleteCartCommandValidator _validator;
    private readonly Faker _faker;

    public DeleteCartCommandValidatorTests()
    {
        _validator = new DeleteCartCommandValidator();
        _faker = new Faker();
    }

    [Fact]
    public async Task DeleteCartCommandValidator_Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var cartId = _faker.Random.Number(1, 100);
        var existingCart = new Cart 
        { 
            Id = cartId, 
            UserId = _faker.Random.Number(),
            Active = true
        };

        var command = new DeleteCartCommand(cartId);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task DeleteCartCommandValidator_Should_Fail_When_CartId_Is_Empty()
    {
        // Arrange
        var command = new DeleteCartCommand(0);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id);
    }
} 