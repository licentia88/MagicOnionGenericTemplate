using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

 
/// <summary>
/// Service for querying audit data with encryption and authorization.
/// </summary>
// ReSharper disable once UnusedType.Global
public class AuditQueryService : MagicServerSecureService<IAuditQueryService, AUDIT_QUERY, MagicTContext>, IAuditQueryService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuditQueryService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuditQueryService(IServiceProvider provider) : base(provider)
    {
    }

     
}