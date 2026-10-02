using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// A driver authorized under a specific policy. Like Vehicle, a Driver row
/// represents "a named driver on a specific policy", not a global person.
/// The same human on two policies gets two Driver rows, which is correct
/// because their risk profile and coverage may differ per policy.
/// </summary>
public class Driver: BaseEntity
{ 
    public string FullName { get; set; } = string.Empty;
    
    // <summary>
    /// Driving licence number. Unique within a policy.
    /// </summary>
    public string LicenceNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Date of birth. Important for underwriting and risk calculation.
    /// Stored as DateOnly (birthdays have no time component).
    /// </summary>
    public DateOnly DateOfBirth { get; set; }
    
    /// <summary>
    /// Years of driving experience. Could be computed from licence issue date
    /// in a later iteration, but storing it explicitly allows for
    /// manual correction when the business needs to override.
    /// </summary>
    public int YearsOfExperience { get; set; }
    
    /// <summary>
    /// Whether this is the primary driver on the policy.
    /// A policy can have many drivers, but typically one primary.
    /// Enforced as "at most one primary per policy" in the Application layer.
    /// </summary>
    public bool IsPrimaryDriver { get; set; }
    
    /// <summary>
    /// Date the driver was added to the policy.
    /// </summary>
    public DateOnly EffectiveFrom { get; set; }
    
    /// <summary>
    /// Date the driver was removed from the policy, if applicable.
    /// Null means still authorized.
    /// </summary>
    public DateOnly? EffectiveTo { get; set; }
    
    
    // --- Relationships ---
    public Guid PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    public ICollection<VehicleDriver> VehicleDrivers { get; set; } = new List<VehicleDriver>();

    public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    
    
    
}