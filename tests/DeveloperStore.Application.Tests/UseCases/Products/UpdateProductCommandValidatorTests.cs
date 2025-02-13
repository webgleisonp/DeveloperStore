using Bogus;
using DeveloperStore.Application.Usecases.Products;
using DeveloperStore.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace DeveloperStore.Application.Tests.UseCases.Products;

public class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator;
    private readonly Faker _faker;

    public UpdateProductCommandValidatorTests()
    {
        _validator = new UpdateProductCommandValidator();
        _faker = new Faker();
    }

    [Fact]
    public void Validate_Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Id: 1,
            Title: _faker.Commerce.ProductName(),
            Price: _faker.Random.Decimal(1, 1000),
            Description: _faker.Lorem.Sentence(),
            Category: _faker.Commerce.Categories(1)[0],
            Image: _faker.Image.PicsumUrl(),
            Rating: new Rating(4.5m, 100)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_Should_Fail_When_Id_Is_Empty()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Id: 0, // Id inválido
            Title: _faker.Commerce.ProductName(),
            Price: _faker.Random.Decimal(1, 1000),
            Description: _faker.Lorem.Sentence(),
            Category: _faker.Commerce.Categories(1)[0],
            Image: _faker.Image.PicsumUrl(),
            Rating: new Rating(4.5m, 100)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id);
    }

    [Fact]
    public void Validate_Should_Fail_When_Title_Is_Empty()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Id: 1,
            Title: "", // Título vazio
            Price: _faker.Random.Decimal(1, 1000),
            Description: _faker.Lorem.Sentence(),
            Category: _faker.Commerce.Categories(1)[0],
            Image: _faker.Image.PicsumUrl(),
            Rating: new Rating(4.5m, 100)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Title);
    }

    [Fact]
    public void Validate_Should_Fail_When_Price_Is_Zero_Or_Negative()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Id: 1,
            Title: _faker.Commerce.ProductName(),
            Price: 0, // Preço inválido
            Description: _faker.Lorem.Sentence(),
            Category: _faker.Commerce.Categories(1)[0],
            Image: _faker.Image.PicsumUrl(),
            Rating: new Rating(4.5m, 100)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Price);
    }

    [Fact]
    public void Validate_Should_Fail_When_Description_Is_Empty()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Id: 1,
            Title: _faker.Commerce.ProductName(),
            Price: _faker.Random.Decimal(1, 1000),
            Description: "",
            Category: _faker.Commerce.Categories(1)[0],
            Image: _faker.Image.PicsumUrl(),
            Rating: new Rating(4.5m, 100)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Fact]
    public void Validate_Should_Fail_When_Category_Is_Empty()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Id: 1,
            Title: _faker.Commerce.ProductName(),
            Price: _faker.Random.Decimal(1, 1000),
            Description: _faker.Lorem.Sentence(),
            Category: "",
            Image: _faker.Image.PicsumUrl(),
            Rating: new Rating(4.5m, 100)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Category);
    }

    [Fact]
    public void Validate_Should_Fail_When_Image_Is_Empty()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Id: 1,
            Title: _faker.Commerce.ProductName(),
            Price: _faker.Random.Decimal(1, 1000),
            Description: _faker.Lorem.Sentence(),
            Category: _faker.Commerce.Categories(1)[0],
            Image: "",
            Rating: new Rating(4.5m, 100)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Image);
    }
}
