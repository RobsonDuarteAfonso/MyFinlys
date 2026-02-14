using Moq;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Tests.Application.Services;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly Mock<IBankRepository> _mockBankRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockBankRepository = new Mock<IBankRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        
        _accountService = new AccountService(
            _mockAccountRepository.Object,
            _mockBankRepository.Object,
            _mockUserRepository.Object
        );
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentBank_ShouldThrowException()
    {
        // Arrange
        var bankId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new AccountCreateDto
        {
            Number = "123456",
            Type = "Savings",
            BankId = bankId,
            UserIds = [userId]
        };

        _mockBankRepository.Setup(r => r.GetByIdAsync(bankId))
            .ReturnsAsync((Bank?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _accountService.CreateAsync(dto)
        );
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentUser_ShouldThrowException()
    {
        // Arrange
        var bankId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var bank = Bank.Create("Test Bank");

        var dto = new AccountCreateDto
        {
            Number = "123456",
            Type = "Savings",
            BankId = bankId,
            UserIds = [userId]
        };

        _mockBankRepository.Setup(r => r.GetByIdAsync(bankId))
            .ReturnsAsync(bank);
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _accountService.CreateAsync(dto)
        );
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnAccount()
    {
        // Arrange
        var bankId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var bank = Bank.Create("Test Bank");
        var account = Account.Create("123456", AccountType.Savings, bankId);
        
        // Set Bank property on account (simulating EF navigation property loading)
        var accountProperty = typeof(Account).GetProperty("Bank", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        accountProperty?.SetValue(account, bank);

        _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        // Act
        var result = await _accountService.GetByIdAsync(accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(account.Number, result.Number);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync((Account?)null);

        // Act
        var result = await _accountService.GetByIdAsync(accountId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = Account.Create("123456", AccountType.Savings, Guid.NewGuid());

        _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);
        _mockAccountRepository.Setup(r => r.DeleteAsync(accountId))
            .Returns(Task.CompletedTask);
        _mockAccountRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _accountService.DeleteAsync(accountId);

        // Assert
        Assert.True(result);
        _mockAccountRepository.Verify(r => r.DeleteAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync((Account?)null);

        // Act
        var result = await _accountService.DeleteAsync(accountId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAccounts()
    {
        // Arrange
        var bank = Bank.Create("Test Bank");
        var accounts = new[]
        {
            Account.Create("123456", AccountType.Savings, bank.Id),
            Account.Create("654321", AccountType.Checking, bank.Id)
        };
        
        // Set Bank property on accounts (simulating EF navigation property loading)
        var accountProperty = typeof(Account).GetProperty("Bank", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var account in accounts)
        {
            accountProperty?.SetValue(account, bank);
        }

        _mockAccountRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(accounts);

        // Act
        var result = await _accountService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
