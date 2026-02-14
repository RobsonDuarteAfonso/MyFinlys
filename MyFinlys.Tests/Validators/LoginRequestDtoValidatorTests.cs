using FluentValidation;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Validators;

namespace MyFinlys.Tests.Validators;

public class LoginRequestDtoValidatorTests
{
    private readonly LoginRequestDtoValidator _validator;

    public LoginRequestDtoValidatorTests()
    {
        _validator = new LoginRequestDtoValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldSucceed()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "SecurePass123"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithEmptyEmail_ShouldFail()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "",
            Password = "SecurePass123"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldFail()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "invalid-email",
            Password = "SecurePass123"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_WithShortPassword_ShouldFail()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "123"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_WithEmptyPassword_ShouldFail()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = ""
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }
}
