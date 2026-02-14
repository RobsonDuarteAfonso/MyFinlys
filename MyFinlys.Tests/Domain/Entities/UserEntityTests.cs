using MyFinlys.Domain.Entities;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Tests.Domain.Entities;

public class UserEntityTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateUser()
    {
        // Arrange
        const string name = "John Doe";
        const string email = "john@example.com";
        const string password = "SecurePass@123";

        // Act
        var user = User.Create(name, email, password);

        // Assert
        Assert.NotNull(user);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(name, user.Name);
        Assert.NotNull(user.Email);
        Assert.NotNull(user.Password);
    }

    [Fact]
    public void Create_WithShortName_ShouldThrowException()
    {
        // Arrange
        const string name = "Jo";
        const string email = "john@example.com";
        const string password = "SecurePass@123";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => User.Create(name, email, password)
        );
        Assert.Contains("at least", exception.Message);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var user = User.Create("John", "john@example.com", "SecurePass@123");
        
        // Act
        var isValid = user.VerifyPassword("SecurePass@123");

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    {
        // Arrange
        var user = User.Create("John", "john@example.com", "SecurePass@123");
        
        // Act
        var isValid = user.VerifyPassword("WrongPassword@123");

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ChangePassword_WithNewPassword_ShouldUpdatePassword()
    {
        // Arrange
        var user = User.Create("John", "john@example.com", "OldPass@123");
        
        // Act
        user.ChangePassword("NewPass@123");

        // Assert
        Assert.True(user.VerifyPassword("NewPass@123"));
        Assert.False(user.VerifyPassword("OldPass@123"));
    }

    [Fact]
    public void SoftDelete_ShouldMarkAsDeleted()
    {
        // Arrange
        var user = User.Create("John", "john@example.com", "SecurePass@123");
        
        // Act
        user.SoftDelete();

        // Assert
        Assert.True(user.IsDeleted);
        Assert.NotNull(user.DeletedAt);
    }

    [Fact]
    public void Restore_AfterSoftDelete_ShouldRestoreUser()
    {
        // Arrange
        var user = User.Create("John", "john@example.com", "SecurePass@123");
        user.SoftDelete();

        // Act
        user.Restore();

        // Assert
        Assert.False(user.IsDeleted);
        Assert.Null(user.DeletedAt);
    }

    [Fact]
    public void SetUpdatedAt_ShouldUpdateTimestamp()
    {
        // Arrange
        var user = User.Create("John", "john@example.com", "SecurePass@123");
        var originalUpdatedAt = user.UpdatedAt;
        
        System.Threading.Thread.Sleep(100);

        // Act
        user.SetUpdatedAt();

        // Assert
        Assert.True(user.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void AddAccount_ShouldAddUserAccount()
    {
        // Arrange
        var user = User.Create("John", "john@example.com", "SecurePass@123");
        var bank = Bank.Create("Test Bank");
        var account = Account.Create("123456", MyFinlys.Domain.Enums.AccountType.Savings, bank.Id);

        // Act
        user.AddAccount(account);

        // Assert
        Assert.Single(user.UserAccounts);
    }
}
