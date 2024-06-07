using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class AuditRecordsDService : MagicServerSecureService<IAuditRecordsDService, AUDIT_RECORDS_D, MagicTContext>, IAuditRecordsDService
{
    public AuditRecordsDService(IServiceProvider provider) : base(provider)
    {
    }
}
