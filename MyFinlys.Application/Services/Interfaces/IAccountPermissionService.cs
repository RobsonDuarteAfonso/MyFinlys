using MyFinlys.Domain.Enums;

namespace MyFinlys.Application.Services.Interfaces;

public interface IAccountPermissionService
{
    Task<bool> HasAccessAsync(Guid userId, Guid accountId, params AccessLevel[] allowedLevels);
    Task<bool> CanModifyUsersAsync(Guid userId, Guid accountId);
}
