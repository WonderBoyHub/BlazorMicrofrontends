namespace BlazorMicrofrontends.Core;

/// <summary>
/// Defines how routes are matched in the MicrofrontendRouter.
/// </summary>
public enum RouteMode
{
    /// <summary>
    /// Routes must match exactly.
    /// </summary>
    Exact,
    
    /// <summary>
    /// Routes are matched by prefix.
    /// </summary>
    Prefix
} 