using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class AuditQueryService : MagicServerService<IAuditQueryService, AUDIT_QUERY>, IAuditQueryService
{
    public AuditQueryService(IServiceProvider provider) : base(provider)
    {
    }
}