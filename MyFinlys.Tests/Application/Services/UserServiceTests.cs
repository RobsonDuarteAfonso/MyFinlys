using Moq;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Tests.Application.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnUserId()
    {
        // Arrange
        const string name = "John Doe";
        const string email = "john@example.com";
        const string password = "SecurePass@123";

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var userId = await _userService.CreateAsync(name, email, password);

        // Assert
        Assert.NotEqual(Guid.Empty, userId);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = User.Create("John", "john@example.com", "SecurePass@123");
        
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Name, result.Name);
        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithCorrectCredentials_ShouldReturnTrue()
    {
        // Arrange
        const string email = "john@example.com";
        const string password = "SecurePass@123";
        var user = User.Create("John", email, password);

        _mockUserRepository.Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.ValidateCredentialsAsync(email, password);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(r => r.GetByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithWrongPassword_ShouldReturnFalse()
    {
        // Arrange
        const string email = "john@example.com";
        const string password = "SecurePass@123";
        var user = User.Create("John", email, password);

        _mockUserRepository.Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.ValidateCredentialsAsync(email, "WrongPassword");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithNonExistentEmail_ShouldReturnFalse()
    {
        // Arrange
        const string email = "nonexistent@example.com";
        const string password = "SecurePass@123";

        _mockUserRepository.Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.ValidateCredentialsAsync(email, password);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = User.Create("John", "john@example.com", "SecurePass@123");

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.DeleteAsync(userId))
            .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.DeleteAsync(userId);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(r => r.DeleteAsync(userId), Times.Once);
        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.DeleteAsync(userId);

        // Assert
        Assert.False(result);
        _mockUserRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithCorrectCurrentPassword_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string currentPassword = "OldPass@123";
        const string newPassword = "NewPass@123";
        var user = User.Create("John", "john@example.com", currentPassword);

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithWrongCurrentPassword_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string currentPassword = "OldPass@123";
        const string newPassword = "NewPass@123";
        var user = User.Create("John", "john@example.com", currentPassword);

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.ChangePasswordAsync(userId, "WrongPassword", newPassword);

        // Assert
        Assert.False(result);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}
