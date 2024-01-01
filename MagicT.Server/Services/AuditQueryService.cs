using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;
public class AuditQueryService : MagicServerAuthService<IAuditQueryService, AUDIT_QUERY,MagicTContext>, IAuditQueryService
{
    public AuditQueryService(IServiceProvider provider) : base(provider)
    {
    }

 
}