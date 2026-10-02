using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// A claim filed against a policy. A claim always belongs to a Policy,
/// and usually (but not always) references a specific Vehicle and Driver.
/// If the vehicle or driver is unknown at filing time, those FKs are null
/// and are filled in later during assessment.
/// </summary>
public class Claim: BaseEntity
{ /// <summary>
    /// Human-readable claim reference, e.g., "CLM-2026-0001".
    /// Unique across all claims.
    /// </summary>
    public string ClaimNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Date and time the incident occurred. Uses DateTimeOffset because
    /// the incident happens at a specific moment in a specific place —
    /// the timezone offset matters for accident reconstruction.
    /// </summary>
    public DateTimeOffset IncidentDate { get; set; }
    
    /// <summary>
    /// Date and time the claim was filed. Usually close to IncidentDate
    /// but can be days or weeks later.
    /// </summary>
    public DateTimeOffset FiledDate { get; set; }
    
    /// <summary>
    /// Free-text description of what happened.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    
    /// <summary>
    /// Estimated or assessed claim amount.
    /// Stored as decimal — money must never be float/double.
    /// </summary>
    public decimal ClaimAmount { get; set; }
    
    /// <summary>
    /// Amount actually approved for payout, if any.
    /// Null until the claim reaches Approved or Paid status.
    /// </summary>
    public decimal? ApprovedAmount { get; set; }
    
    // <summary>
    /// Current lifecycle status.
    /// </summary>
    public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
    
    // --- Relationships ---

    public Guid PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    /// <summary>
    /// Optional reference to the vehicle involved. Nullable because the
    /// vehicle may not be identified at filing time (e.g., theft).
    /// </summary>
    public Guid? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    /// <summary>
    /// Optional reference to the driver involved.
    /// </summary>
    public Guid? DriverId { get; set; }
    public Driver? Driver { get; set; }
    
}