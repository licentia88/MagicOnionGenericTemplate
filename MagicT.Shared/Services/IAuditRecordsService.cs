using MagicOnion;
using MagicT.Shared.Models;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

public interface IAuditRecordsService : IMagicSecureService<IAuditRecordsService, AUDIT_RECORDS>
{
    UnaryResult<AUDIT_RECORDS> GetDataLogs(string TableName, int PrimaryKey);
}
