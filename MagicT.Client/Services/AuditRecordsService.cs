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

    public UnaryResult<AUDIT_RECORDS> GetDataLogs(string TableName, int PrimaryKey)
    {
        return Client.GetDataLogs(TableName, PrimaryKey);
    }
}

