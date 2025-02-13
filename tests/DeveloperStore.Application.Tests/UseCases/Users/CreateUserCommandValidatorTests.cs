using Bogus;
using DeveloperStore.Application.Usecases.Users;
using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace DeveloperStore.Application.Tests.UseCases.Users;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator;
    private readonly Faker _faker;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
        _faker = new Faker();
    }

    [Fact]
    public void Validate_Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: _faker.Internet.Email(),
            UserName: "ValidUser",
            Password: "ValidPass1@",
            Name: new Name("John", "Doe"),
            Address: new Address("Street", "City", 20, "12345", _faker.Address.Latitude().ToString(), _faker.Address.Latitude().ToString()),
            Phone: _faker.Phone.PhoneNumber(),
            Status: Status.Active,
            Role: Role.Admin
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void Validate_Should_Fail_When_Email_Is_Invalid(string email)
    {
        var command = new CreateUserCommand(
            Email: email,
            UserName: "ValidUser",
            Password: "ValidPass1@",
            Name: new Name("John", "Doe"),
            Address: new Address("Street", "City", 20, "12345", _faker.Address.Latitude().ToString(), _faker.Address.Latitude().ToString()),
            Phone: "1234567890",
            Status: Status.Active,
            Role: Role.Admin
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("thisIsWayTooLongUserName")]
    public void Validate_Should_Fail_When_UserName_Is_Invalid(string userName)
    {
        var command = new CreateUserCommand(
            Email: "user@example.com",
            UserName: userName,
            Password: "ValidPass1@",
            Name: new Name("John", "Doe"),
            Address: new Address("Street", "City", 20, "12345", _faker.Address.Latitude().ToString(), _faker.Address.Latitude().ToString()),
            Phone: "1234567890",
            Status: Status.Active,
            Role: Role.Admin
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("short1")]
    [InlineData("ThisIsTooLongPassword123")]
    public void Validate_Should_Fail_When_Password_Is_Invalid(string password)
    {
        var command = new CreateUserCommand(
            Email: "user@example.com",
            UserName: "ValidUser",
            Password: password,
            Name: new Name("John", "Doe"),
            Address: new Address("Street", "City", 20, "12345", _faker.Address.Latitude().ToString(), _faker.Address.Latitude().ToString()),
            Phone: "1234567890",
            Status: Status.Active,
            Role: Role.Admin
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public void Validate_Should_Fail_When_Name_Is_Null()
    {
        var command = new CreateUserCommand(
            Email: "user@example.com",
            UserName: "ValidUser",
            Password: "ValidPass1@",
            Name: null,
            Address: new Address("Street", "City", 20, "12345", _faker.Address.Latitude().ToString(), _faker.Address.Latitude().ToString()),
            Phone: "1234567890",
            Status: Status.Active,
            Role: Role.Admin
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_Should_Fail_When_Address_Is_Null()
    {
        var command = new CreateUserCommand(
            Email: "user@example.com",
            UserName: "ValidUser",
            Password: "ValidPass1@",
            Name: new Name("John", "Doe"),
            Address: null,
            Phone: "1234567890",
            Status: Status.Active,
            Role: Role.Admin
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Address);
    }

    [Fact]
    public void Validate_Should_Fail_When_Phone_Is_Empty()
    {
        var command = new CreateUserCommand(
            Email: "user@example.com",
            UserName: "ValidUser",
            Password: "ValidPass1@",
            Name: new Name("John", "Doe"),
            Address: new Address("Street", "City", 20, "12345", _faker.Address.Latitude().ToString(), _faker.Address.Latitude().ToString()),
            Phone: "",
            Status: Status.Active,
            Role: Role.Admin
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Phone);
    }
}
