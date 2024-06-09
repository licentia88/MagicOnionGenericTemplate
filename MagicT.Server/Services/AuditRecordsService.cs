using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

public class AuditRecordsService : MagicServerSecureService<IAuditRecordsService, AUDIT_RECORDS, MagicTContext>, IAuditRecordsService
{
    public AuditRecordsService(IServiceProvider provider) : base(provider)
    {
    }

    public  async UnaryResult<List<AUDIT_RECORDS>> GetRecordLogsAsync(string TableName, string PrimaryKeyValue)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Db.AUDIT_RECORDS
               .Where(x => x.AR_TABLE_NAME == TableName && x.AR_PK_VALUE == PrimaryKeyValue)
               .OrderByDescending(x => x.AB_DATE).AsNoTracking().ToListAsync();

            return result;
         });
    }
}
