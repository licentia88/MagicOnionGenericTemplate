using MagicT.Server.Enums;
using MagicT.Shared.Models;

namespace MagicT.Server.Payloads;

/// <summary>
/// Represents the payload for a failed audit operation.
/// </summary>
public class AuditFailedPayload
{
    /// <summary>
    /// Gets the audit query details for the failed audit.
    /// </summary>
    public AUDIT_FAILED AuditQuery { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditFailedPayload"/> class.
    /// </summary>
    /// <param name="userId">The user ID associated with the audit.</param>
    /// <param name="error">The error message for the failed audit.</param>
    /// <param name="service">The service name where the audit failed.</param>
    /// <param name="method">The method name where the audit failed.</param>
    /// <param name="endPoint">The endpoint associated with the audit.</param>
    /// <param name="parameters">The parameters associated with the audit.</param>
    public AuditFailedPayload(int userId, string error, string service, string method, string endPoint, string parameters)
    {
        AuditQuery = new AUDIT_FAILED
        {
            AB_USER_ID = userId,
            AB_METHOD = method,
            AB_SERVICE = service,
            AB_DATE = DateTime.Now,
            AB_TYPE = (int)AuditType.Error,
            AB_END_POINT = endPoint,
            AF_PARAMETERS = parameters,
            AF_ERROR = error,
        };
    }
}