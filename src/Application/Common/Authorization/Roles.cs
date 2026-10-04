namespace Application.Common.Authorization;

/// <summary>
/// Role names recognized by the application.
/// Kept as constants so that:
/// 1. There is a single source of truth for role strings.
/// 2. Refactoring a role name is a compile-time error, not a runtime bug.
/// 3. Infrastructure can seed exactly the roles Application expects.
/// </summary>
public static class Roles
{   //manages users, roles, and system configuration. The only role that can create or delete users.
    public const string Admin = "Admin";
    
    //creates and manages policies, vehicles, drivers. Owns the "risk pricing and issuance" side of the business.
    public const string Underwriter = "Underwriter";
    
    //iles, assesses, and resolves claims. Owns the "loss" side of the business.
    public const string ClaimsHandler = "ClaimsHandler";
    
    public static IReadOnlyList<string> AllRoles = new[] { Admin, Underwriter,ClaimsHandler };
}