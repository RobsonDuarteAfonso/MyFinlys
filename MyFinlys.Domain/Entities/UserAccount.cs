using MyFinlys.Domain.Common;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Entities;

public class UserAccount : Entity
{
    public Guid UserId { get; private set; }
    public User User { get; set; } = null!;
    public Guid AccountId { get; private set; }
    public Account Account { get; private set; } = null!;
    public AccessLevel AccessLevel { get; private set; }

    private UserAccount() { }

    private UserAccount(Guid userId, Guid accountId, AccessLevel accessLevel) : base()
    {
        UserId = userId;
        AccountId = accountId;
        AccessLevel = accessLevel;
    }

    public static UserAccount Create(Guid userId, Guid accountId, AccessLevel accessLevel)
    {
        Guard.AgainstEmptyGuid(userId, nameof(userId));
        Guard.AgainstEmptyGuid(accountId, nameof(accountId));
        Guard.AgainstInvalidEnumValue(accessLevel, nameof(accessLevel));

        return new UserAccount(userId, accountId, accessLevel);
    }

    public void UpdateAccessLevel(AccessLevel accessLevel)
    {
        Guard.AgainstInvalidEnumValue(accessLevel, nameof(accessLevel));
        AccessLevel = accessLevel;
    }
}
