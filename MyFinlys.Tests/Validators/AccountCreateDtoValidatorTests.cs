using MyFinlys.Application.DTOs;
using MyFinlys.Application.Validators;

namespace MyFinlys.Tests.Validators;

public class AccountCreateDtoValidatorTests
{
    private readonly AccountCreateDtoValidator _validator;

    public AccountCreateDtoValidatorTests()
    {
        _validator = new AccountCreateDtoValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldSucceed()
    {
        // Arrange
        var dto = new AccountCreateDto
        {
            Number = "123456",
            Type = "Savings",
            BankId = Guid.NewGuid(),
            UserIds = [Guid.NewGuid()]
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithShortNumber_ShouldFail()
    {
        // Arrange
        var dto = new AccountCreateDto
        {
            Number = "12",
            Type = "Savings",
            BankId = Guid.NewGuid(),
            UserIds = [Guid.NewGuid()]
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Number");
    }

    [Fact]
    public void Validate_WithInvalidAccountType_ShouldFail()
    {
        // Arrange
        var dto = new AccountCreateDto
        {
            Number = "123456",
            Type = "InvalidType",
            BankId = Guid.NewGuid(),
            UserIds = [Guid.NewGuid()]
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Type");
    }

    [Fact]
    public void Validate_WithEmptyBankId_ShouldFail()
    {
        // Arrange
        var dto = new AccountCreateDto
        {
            Number = "123456",
            Type = "Savings",
            BankId = Guid.Empty,
            UserIds = [Guid.NewGuid()]
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "BankId");
    }

    [Fact]
    public void Validate_WithNoUsers_ShouldFail()
    {
        // Arrange
        var dto = new AccountCreateDto
        {
            Number = "123456",
            Type = "Savings",
            BankId = Guid.NewGuid(),
            UserIds = []
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UserIds");
    }
}
