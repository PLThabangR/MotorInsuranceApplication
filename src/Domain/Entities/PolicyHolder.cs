using Domain.Common;

namespace Domain.Entities;
// <summary>
/// The legal owner of one or more insurance policies.
/// In a real system this might be a person or a company; for now we model
/// the common attributes and treat it as a natural person.

public class PolicyHolder: BaseEntity
{
    /// <summary>
    /// Full legal name. Kept as a single field for simplicity; a real system
    /// might split into FirstName / LastName, but many insurers intentionally
    /// keep a single "legal name" field to match regulatory documents.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Government-issued identity number (e.g., SA ID, passport).
    /// Must be unique across policyholders — enforced via DB unique index.
    /// </summary>
    public string NationalId { get; set; } = string.Empty;

    /// <summary>
    /// Email address. Used for correspondence and as a unique contact key.
    /// Unique index will be added in TICKET-003.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contact phone number. Format is intentionally free-form at the domain
    /// level; validation belongs in the Application layer (FluentValidation).
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    // --- Address (flattened for simplicity) ---
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    // --- Relationships ---

    /// <summary>
    /// Policies owned by this policyholder. One-to-many.
    /// Initialized to a new list so that code creating a PolicyHolder
    /// does not have to null-check the collection.
    /// </summary>
    public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    
    
}