using Benutomo;
using MagicT.Shared.Models;

namespace MagicT.Server.Payloads;

/// <summary>
/// Represents the payload for an audit query operation.
/// </summary>
[AutomaticDisposeImpl]
public partial class AuditQueryPayload :IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the audit query details.
    /// </summary>
    [EnableAutomaticDispose]
    public AUDIT_QUERY AuditQuery { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditQueryPayload"/> class.
    /// </summary>
    /// <param name="userId">The user ID associated with the audit.</param>
    /// <param name="service">The service name where the audit query is performed.</param>
    /// <param name="method">The method name where the audit query is performed.</param>
    /// <param name="endPoint">The endpoint associated with the audit query.</param>
    /// <param name="parameters">The parameters associated with the audit query.</param>
    public AuditQueryPayload(int userId, string service, string method, string endPoint, string parameters)
    {
        AuditQuery = new AUDIT_QUERY
        {
            AB_USER_ID = userId,
            AB_METHOD = method,
            AB_SERVICE = service,
            AB_DATE = global::System.DateTime.Now,
            AB_TYPE = (int)global::MagicT.Server.Enums.AuditType.Query,
            AB_END_POINT = endPoint,
            AQ_PARAMETERS = parameters,
        };
    }
    ~AuditQueryPayload()
    {
        Dispose(false);
        GC.WaitForPendingFinalizers();
    }
}