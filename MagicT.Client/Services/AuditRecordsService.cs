using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

[RegisterScoped]
public sealed class AuditRecordsService : MagicClientSecureService<IAuditRecordsService, AUDIT_RECORDS>, IAuditRecordsService
{
    public AuditRecordsService(IServiceProvider provider) : base(provider)
    {
    }

    public UnaryResult<List<AUDIT_RECORDS>> GetRecordLogsAsync(string TableName, string PrimaryKeyValue)
    {
        return Client.GetRecordLogsAsync(TableName, PrimaryKeyValue);
    }
}

