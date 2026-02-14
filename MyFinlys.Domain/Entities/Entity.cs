namespace MyFinlys.Domain.Entities;

public abstract class Entity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    protected Entity() { }

    protected Entity(Guid id, DateTime createdAt, DateTime updatedAt, Guid? createdBy = null)
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    public void SetUpdatedAt(Guid? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        if (updatedBy.HasValue)
            UpdatedBy = updatedBy;
    }

    public void SoftDelete(Guid? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        if (deletedBy.HasValue)
            DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;    }
}