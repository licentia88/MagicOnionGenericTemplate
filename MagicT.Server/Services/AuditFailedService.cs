using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class AuditFailedService : MagicServerSecureService<IAuditFailedService, AUDIT_FAILED,MagicTContext>, IAuditFailedService
{
    public AuditFailedService(IServiceProvider provider) : base(provider)
    {
    }
}