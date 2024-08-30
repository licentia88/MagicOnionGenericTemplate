namespace MagicT.Server.Enums;

/// <summary>
/// Represents the types of audit actions that can be performed.
/// </summary>
public enum AuditType
{
    /// <summary>
    /// Represents a query action.
    /// </summary>
    Query = 0,

    /// <summary>
    /// Represents a create action.
    /// </summary>
    Create = 1,

    /// <summary>
    /// Represents an update action.
    /// </summary>
    Update = 2,

    /// <summary>
    /// Represents a delete action.
    /// </summary>
    Delete = 3,

    /// <summary>
    /// Represents an error action.
    /// </summary>
    Error = 4
}