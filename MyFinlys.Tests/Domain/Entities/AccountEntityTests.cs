using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Tests.Domain.Entities;

public class AccountEntityTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateAccount()
    {
        // Arrange
        const string number = "123456";
        var bankId = Guid.NewGuid();

        // Act
        var account = Account.Create(number, AccountType.Savings, bankId);

        // Assert
        Assert.NotNull(account);
        Assert.Equal(number, account.Number);
        Assert.Equal(AccountType.Savings, account.Type);
        Assert.Equal(bankId, account.BankId);
    }

    [Fact]
    public void Create_WithShortNumber_ShouldThrowException()
    {
        // Arrange
        const string number = "12";
        var bankId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => Account.Create(number, AccountType.Savings, bankId)
        );
        Assert.Contains("at least", exception.Message);
    }

    [Fact]
    public void Create_WithEmptyBankId_ShouldThrowException()
    {
        // Arrange
        const string number = "123456";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => Account.Create(number, AccountType.Savings, Guid.Empty)
        );
        Assert.Contains("cannot be empty", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateAccount()
    {
        // Arrange
        var account = Account.Create("123456", AccountType.Savings, Guid.NewGuid());
        var newNumber = "654321";
        var newType = AccountType.Checking;
        var newBankId = Guid.NewGuid();

        // Act
        account.Update(newNumber, newType, newBankId);

        // Assert
        Assert.Equal(newNumber, account.Number);
        Assert.Equal(newType, account.Type);
        Assert.Equal(newBankId, account.BankId);
    }

    [Fact]
    public void AddUser_ShouldAddUserAccount()
    {
        // Arrange
        var account = Account.Create("123456", AccountType.Savings, Guid.NewGuid());
        var user = User.Create("John", "john@example.com", "SecurePass@123");

        // Act
        account.AddUser(user);

        // Assert
        Assert.Single(account.UserAccounts);
    }

    [Fact]
    public void AddBalance_ShouldAddBalance()
    {
        // Arrange
        var account = Account.Create("123456", AccountType.Savings, Guid.NewGuid());
        var balance = Balance.Create(account.Id, Year.Create(2024), MyFinlys.Domain.Enums.Month.January, 1000);

        // Act
        account.AddBalance(balance);

        // Assert
        Assert.Single(account.Balances);
    }

    [Fact]
    public void SoftDelete_ShouldMarkAsDeleted()
    {
        // Arrange
        var account = Account.Create("123456", AccountType.Savings, Guid.NewGuid());

        // Act
        account.SoftDelete();

        // Assert
        Assert.True(account.IsDeleted);
        Assert.NotNull(account.DeletedAt);
    }
}
