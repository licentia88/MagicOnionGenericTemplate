using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

/// <summary>
/// Service for handling audit records data with encryption and authorization.
/// </summary>
// ReSharper disable once UnusedType.Global
public class AuditRecordsService : MagicServerSecureService<IAuditRecordsService, AUDIT_RECORDS, MagicTContext>, IAuditRecordsService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuditRecordsService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuditRecordsService(IServiceProvider provider) : base(provider)
    {
    }

    /// <summary>
    /// Gets the audit record logs asynchronously based on the table name and primary key value.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="primaryKeyValue">The value of the primary key.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the list of audit records.</returns>
    public async UnaryResult<List<AUDIT_RECORDS>> GetRecordLogsAsync(string tableName, string primaryKeyValue)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Db.AUDIT_RECORDS
               .Where(x => x.AR_TABLE_NAME == tableName && x.AR_PK_VALUE == primaryKeyValue)
               .OrderByDescending(x => x.AB_DATE).AsNoTracking().ToListAsync();

            return result;
         });
    }
}