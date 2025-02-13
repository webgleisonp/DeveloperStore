using DeveloperStore.Application.Usecases.Products;
using FluentValidation.TestHelper;

namespace DeveloperStore.Application.Tests.UseCases.Products;

public class DeleteProductCommandValidatorTests
{
    private readonly DeleteProductCommandValidator _validator;

    public DeleteProductCommandValidatorTests()
    {
        _validator = new DeleteProductCommandValidator();
    }

    [Fact]
    public void DeleteProductCommandValidator_Should_Pass_When_Id_Is_Valid()
    {
        // Arrange
        var command = new DeleteProductCommand(1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DeleteProductCommandValidator_Should_Fail_When_Id_Is_Empty()
    {
        // Arrange
        var command = new DeleteProductCommand(0);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id);
    }
}
