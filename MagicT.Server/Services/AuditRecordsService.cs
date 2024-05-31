using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class AuditRecordsService : MagicServerSecureService<IAuditRecordsService, AUDIT_RECORDS,MagicTContext>, IAuditRecordsService
{
    public AuditRecordsService(IServiceProvider provider) : base(provider)
    {
    }
}
 