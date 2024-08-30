using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

/// <summary>
/// Service for handling audit records data with encryption and authorization.
/// </summary>
public class AuditRecordsDService : MagicServerSecureService<IAuditRecordsDService, AUDIT_RECORDS_D, MagicT.Server.Database.MagicTContext>, IAuditRecordsDService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuditRecordsDService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuditRecordsDService(IServiceProvider provider) : base(provider)
    {
    }
}