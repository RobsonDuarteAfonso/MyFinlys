using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : UserMapper.ToDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user is null ? null : UserMapper.ToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(UserMapper.ToDto);
    }

    public async Task<Guid> CreateAsync(string name, string email, string password)
    {
        var user = User.Create(name, email, password);
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return user.Id;
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UserUpdateDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return null;

        user.Update(dto.Name, dto.Email);
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
        return UserMapper.ToDto(user);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing is null) return false;

        await _userRepository.DeleteAsync(id);
        await _userRepository.SaveChangesAsync();
        return true;
    }    

    public async Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null) return false;

        return user.VerifyPassword(password);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) return false;

        if (!user.VerifyPassword(currentPassword))
            return false;

        user.ChangePassword(newPassword);
        await _userRepository.UpdateAsync(user);
        return true;
    }    
}
