using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Explicit join entity for the many-to-many relationship between Vehicle and Driver.
///
/// Why explicit instead of EF Core's implicit join table?
/// 1. The relationship carries additional data (IsPrimaryDriver, effective dates).
/// 2. Insurance needs history — "who was authorized to drive which vehicle, when?"
/// 3. It is queryable and understandable in code and in the database.
///
/// The combination (VehicleId, DriverId, EffectiveFrom) should be unique 
/// </summary>
public class VehicleDriver : BaseEntity
{
    /// <summary>
    /// Whether this driver is the primary driver of this specific vehicle.
    /// Distinct from Driver.IsPrimaryDriver, which is policy-level.
    /// A driver can be primary for one vehicle and secondary for another.
    /// </summary>
    public bool IsPrimaryDriverForVehicle { get; set; }
    
    
    // <summary>
    /// Date this driver was authorized to drive this vehicle.
    /// </summary>
    public DateOnly EffectiveFrom { get; set; }

    /// <summary>
    /// Date this authorization ended, if applicable.
    /// </summary>
    public DateOnly? EffectiveTo { get; set; }
    
    // --- Relationships ---

    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public Guid DriverId { get; set; }
    public Driver Driver { get; set; } = null!;

    /// <summary>
    /// Denormalized PolicyId to allow fast queries like
    /// "all vehicle-driver authorizations on this policy" without a join.
    /// Must be kept consistent with Vehicle.PolicyId and Driver.PolicyId.
    /// Invariant enforced in the Application layer.
    /// </summary>
    public Guid PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;
    
}