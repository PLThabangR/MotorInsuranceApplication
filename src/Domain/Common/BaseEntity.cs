namespace Domain.Common;

/// Base class for all persisted entities in the domain.
/// /// The purpose of this class is to avoid repeating Id/CreatedAt/UpdatedAt
/// /// in every entity, nothing more.
public abstract class BaseEntity
{    /// Primary key. We use Guid rather than int because:
    /// 1. IDs exposed in URLs cannot be enumerated (security).
    /// 2. IDs can be generated client-side or before the DB insert.
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// UTC timestamp when the row was created. Set by the application
    /// or by EF Core interceptors in the Infrastructure layer.
    /// Always UTC — never local time — to avoid timezone bugs.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp of the last update. Nullable because newly created
    /// rows have never been updated. Set by EF Core interceptors on SaveChanges.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}