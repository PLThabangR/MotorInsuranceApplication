namespace Domain.Enums;

//Represent the state of policy
public enum PolicyStatus
{ 
    // Policy has been created but cover has not yet started.
    Pending=0,
    /// Policy is currently in force and cover is active.
    Active=1,
    // Policy term ended naturally (reached EndDate).
    Expired=2,
    // Policy was terminated early by the policyholder or the insurer.
    Cancelled=3,
    /// Policy was cancelled due to non-payment of premium.
    Lapsed=4
    
    
}