using DeveloperStore.Application.Usecases.Authentication;
using FluentValidation.TestHelper;

namespace DeveloperStore.Application.Tests.UseCases.Authentication;

public class AuthCommandValidatorTests
{
    private readonly AuthCommandValidator _validator;

    public AuthCommandValidatorTests()
    {
        _validator = new AuthCommandValidator();
    }

    [Fact]
    public void AuthCommandValidator_Should_Pass_When_UserName_And_Password_Are_Filled()
    {
        // Arrange
        var command = new AuthCommand("user123", "securePassword!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.UserName);
        result.ShouldNotHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public void AuthCommandValidator_Should_Fail_When_UserName_Is_Empty()
    {
        // Arrange
        var command = new AuthCommand("", "securePassword!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UserName);
    }

    [Fact]
    public void AuthCommandValidator_Should_Fail_When_Password_Is_Empty()
    {
        // Arrange
        var command = new AuthCommand("user123", "");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public void AuthCommandValidator_Should_Fail_When_Both_Fields_Are_Empty()
    {
        // Arrange
        var command = new AuthCommand("", "");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UserName);
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}