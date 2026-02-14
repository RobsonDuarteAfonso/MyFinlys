using MyFinlys.Domain.Entities;

namespace MyFinlys.Tests.Domain.Entities;

public class BankEntityTests
{
    [Fact]
    public void Create_WithValidName_ShouldCreateBank()
    {
        // Arrange
        const string name = "Test Bank";

        // Act
        var bank = Bank.Create(name);

        // Assert
        Assert.NotNull(bank);
        Assert.Equal(name, bank.Name);
        Assert.NotEqual(Guid.Empty, bank.Id);
    }

    [Fact]
    public void Create_WithShortName_ShouldThrowException()
    {
        // Arrange
        const string name = "AB";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => Bank.Create(name)
        );
        Assert.Contains("at least", exception.Message);
    }

    [Fact]
    public void Update_WithValidName_ShouldUpdateBank()
    {
        // Arrange
        var bank = Bank.Create("Old Name");
        const string newName = "New Name";

        // Act
        bank.Update(newName);

        // Assert
        Assert.Equal(newName, bank.Name);
    }

    [Fact]
    public void SoftDelete_ShouldMarkAsDeleted()
    {
        // Arrange
        var bank = Bank.Create("Test Bank");

        // Act
        bank.SoftDelete();

        // Assert
        Assert.True(bank.IsDeleted);
        Assert.NotNull(bank.DeletedAt);
    }
}
