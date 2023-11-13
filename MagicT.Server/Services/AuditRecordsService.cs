using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class AuditRecordsService : MagicServerService<IAuditRecordsService, AUDIT_RECORDS>, IAuditRecordsService
{
    public AuditRecordsService(IServiceProvider provider) : base(provider)
    {
    }
}