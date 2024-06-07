using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class AuditRecordsService : MagicServerSecureService<IAuditRecordsService, AUDIT_RECORDS, MagicTContext>, IAuditRecordsService
{
    public AuditRecordsService(IServiceProvider provider) : base(provider)
    {
    }

    public UnaryResult<AUDIT_RECORDS> GetDataLogs(string TableName, int PrimaryKey)
    {
        return new UnaryResult<AUDIT_RECORDS>();
    }
}
