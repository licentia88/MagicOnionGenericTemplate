using MagicOnion;
using MagicT.Server.Database;
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

    /// <summary>
    /// Finds audit queries by parameters asynchronously.
    /// </summary>
    /// <param name="parameters">The parameters to search by.</param>
    /// <returns>A <see cref="UnaryResult{List{AUDIT_QUERY}}"/> containing the list of audit queries.</returns>
    public override async UnaryResult<List<AUDIT_QUERY>> FindByParametersAsync(byte[] parameters)
    {
        var queryData = QueryManager.BuildQuery<AUDIT_QUERY>(parameters);

        var result = await Db.Manager().QueryAsync<AUDIT_QUERY>(queryData.query, queryData.parameters);

        return result;
    }
}