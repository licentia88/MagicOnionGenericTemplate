using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

/// <summary>
/// Service for handling failed audit data with encryption and authorization.
/// </summary>
public class AuditFailedService : MagicServerSecureService<IAuditFailedService, AUDIT_FAILED, MagicTContext>, IAuditFailedService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuditFailedService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuditFailedService(IServiceProvider provider) : base(provider)
    {
    }
}