using MyFinlys.Domain.Common;

namespace MyFinlys.Domain.Entities;

public class UserAccount : Entity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Guid AccountId { get; private set; }
    public Account Account { get; private set; } = null!;

    private UserAccount() { }

    private UserAccount(Guid userId, Guid accountId) : base()
    {
        UserId = userId;
        AccountId = accountId;
    }

    public static UserAccount Create(Guid userId, Guid accountId)
    {
        Guard.AgainstEmptyGuid(userId, nameof(userId));
        Guard.AgainstEmptyGuid(accountId, nameof(accountId));

        return new UserAccount(userId, accountId);
    }
}
