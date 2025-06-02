namespace MyFinlys.Domain.Entities;

public abstract class Entity
{
    public Guid Id { get; private set; }
    public DateTime Created_At { get; private set; }
    public DateTime Updated_At { get; private set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        Created_At = DateTime.UtcNow;
        Updated_At = DateTime.UtcNow;
    }

    public void SetUpdatedAt()
    {
        Updated_At = DateTime.UtcNow;
    }   
} 



