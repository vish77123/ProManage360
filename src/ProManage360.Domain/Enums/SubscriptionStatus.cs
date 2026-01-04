/// <summary>
/// Subscription status
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>Trial period active</summary>
    Trial = 0,

    /// <summary>Active paid subscription</summary>
    Active = 1,

    /// <summary>Payment failed - grace period</summary>
    PastDue = 2,

    /// <summary>Suspended by admin</summary>
    Suspended = 3,

    /// <summary>Cancelled by user</summary>
    Cancelled = 4
}