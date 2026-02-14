using MyFinlys.Application.DTOs;
using MyFinlys.Application.Validators;

namespace MyFinlys.Tests.Validators;

public class UserCreateDtoValidatorTests
{
    private readonly UserCreateDtoValidator _validator;

    public UserCreateDtoValidatorTests()
    {
        _validator = new UserCreateDtoValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldSucceed()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = "SecurePass123"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithShortName_ShouldFail()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Name = "Jo",
            Email = "john@example.com",
            Password = "SecurePass123"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldFail()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Name = "John Doe",
            Email = "invalid-email",
            Password = "SecurePass123"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("weak")]
    [InlineData("UPPERCASE")]
    [InlineData("lowercase")]
    [InlineData("123456")]
    public void Validate_WithWeakPassword_ShouldFail(string password)
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = password
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_WithValidStrongPassword_ShouldSucceed()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = "StrongPass123"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }
}
