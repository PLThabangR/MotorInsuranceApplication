namespace Domain.Enums;

/// <summary>
/// Represents the lifecycle state of a claim.
/// Claim workflows in insurance are regulated and heavily audited;
/// each status transition should later be logged (see future Audit ticket).
/// </summary>
public enum ClaimStatus
{
    /// <summary>
    /// Claim has been filed by the policyholder or staff but not yet triaged.
    /// </summary>
    Submitted = 0,

    /// <summary>
    /// Claim is being assessed by a claims handler.
    /// </summary>
    UnderReview = 1,

    /// <summary>
    /// Additional information is required from the policyholder.
    /// </summary>
    AwaitingInformation = 2,

    /// <summary>
    /// Claim has been approved and payment is pending.
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Claim has been rejected.
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// Claim has been paid out and closed.
    /// </summary>
    Paid = 5,

    /// <summary>
    /// Claim was withdrawn by the policyholder before resolution.
    /// </summary>
    Withdrawn = 6
}
