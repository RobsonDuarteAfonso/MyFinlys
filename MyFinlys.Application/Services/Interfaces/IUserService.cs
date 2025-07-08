using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<Guid> CreateAsync(string name, string email, string password);
    Task<UserDto?> UpdateAsync(Guid id, UserUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ValidateCredentialsAsync(string email, string password);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}
