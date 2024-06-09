using MagicOnion;
using MagicT.Shared.Models;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

public interface IAuditRecordsService : IMagicSecureService<IAuditRecordsService, AUDIT_RECORDS>
{
    UnaryResult<List<AUDIT_RECORDS>> GetRecordLogsAsync(string TableName, string PrimaryKeyValue);
}
