using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents an insurance contract covering one or more vehicles
/// and one or more authorized drivers, owned by a single PolicyHolder.
/// A policy term is a specific period; renewals create new Policy rows
/// so that history is preserved.
/// </summary>
public class Policy:BaseEntity
{
    // <summary>
    /// Human-readable policy number, e.g., "POL-2026-0001".
    /// Unique across all policies — enforced via DB unique index.
    /// Kept separate from Id because customers and staff refer to the
    /// policy by this number, not by the database Guid.
    /// </summary>
    public string PolicyNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Start of cover (inclusive). Stored as a date, not a timestamp,
    /// because insurance terms are defined by calendar days.
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// End of cover (inclusive).
    /// </summary>
    public DateOnly EndDate { get; set; }
    
    /// <summary>
    /// Total premium for the policy term. Always non-negative.
    /// Stored as decimal to avoid floating-point rounding errors.
    /// </summary>
    /// This is a monthly payment by user
    public decimal Premium { get; set; }

    /// <summary>
    /// Current lifecycle status of the policy.
    /// </summary>
    public PolicyStatus Status { get; set; } = PolicyStatus.Pending;
    
    // --- Relationships ---

    /// <summary>
    /// Foreign key to the owning PolicyHolder.
    /// </summary>
    public Guid PolicyHolderId { get; set; }

    public PolicyHolder PolicyHolder { get; set; } = null!;
    
    /// <summary>
    /// Vehicles covered by this policy. One-to-many.
    /// </summary>
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    /// <summary>
    /// Drivers authorized under this policy. One-to-many.
    /// </summary>
    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    
    // <summary>
    /// Claims filed against this policy. One-to-many.
    /// </summary>
    public ICollection<Claim> Claims { get; set; } = new List<Claim>();

    // <summary>
    /// Vehicle-driver associations for this policy.
    /// This collection exists because VehicleDriver is a first-class entity,
    /// not just a hidden EF Core join table — see the class for reasoning.
    /// </summary>
    public ICollection<VehicleDriver> VehicleDrivers { get; set; } = new List<VehicleDriver>();
}