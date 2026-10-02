using Domain.Common;

namespace Domain.Entities;
// <summary>
/// A vehicle covered by a policy. Note: a Vehicle row represents
/// "a specific vehicle on a specific policy", not the physical car over time.
/// If the same physical car is insured under a new policy later, it gets a
/// new Vehicle row. This is a deliberate simplification 
/// </summary>
public class Vehicle :BaseEntity
{
    
    // <summary>
    /// Vehicle registration / licence plate number.
    /// Unique within an active policy but not globally (plates are reused
    /// across countries and over time). Unique index will be composite:
    /// (PolicyId, RegistrationNumber)
    /// </summary>
    public string RegistrationNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Vehicle Identification Number (VIN). Globally unique for the physical car.
    /// 17-character code. We keep it as a string; validation goes in Application.
    /// </summary>
    public string Vin { get; set; } = string.Empty;

    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;
    
    /// <summary>
    /// Year of manufacture.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Optional colour. Free-form; not business-critical.
    /// </summary>
    public string? Colour { get; set; }
    
    // <summary>
    /// Vehicle class affects risk (e.g., sedan, SUV, truck, motorcycle).
    /// Kept as a string for now; if the business needs a fixed vocabulary,
    /// we promote this to an enum in a later ticket.
    /// </summary>
    public string VehicleType { get; set; } = string.Empty;
    
    // <summary>
    /// Date the vehicle was added to the policy.
    /// Insurance needs to know exactly when cover started for this vehicle.
    /// </summary>
    public DateOnly EffectiveFrom { get; set; }
    
    /// <summary>
    /// Date the vehicle was removed from the policy, if applicable.
    /// Null means the vehicle is still on the policy.
    /// We never hard-delete a vehicle — history matters in insurance.
    /// </summary>
    public DateOnly? EffectiveTo { get; set; }
    
    // --- Relationships ---

    public Guid PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    
    public ICollection<VehicleDriver> VehicleDrivers { get; set; } = new List<VehicleDriver>();

    /// <summary>
    /// Telematics readings emitted by this vehicle. Append-only.
    /// Could be thousands or millions of rows over the vehicle's life.
    /// </summary>
    public ICollection<Telematics> Telematics { get; set; } = new List<Telematics>();

    /// <summary>
    /// Claims that reference this vehicle. Optional — a claim may be filed
    /// before the vehicle is confirmed.
    /// </summary>
    public ICollection<Claim> Claims { get; set; } = new List<Claim>();
}