using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

public sealed class AuditQueryService : MagicClientSecureService<IAuditQueryService, AUDIT_QUERY>, IAuditQueryService
{
    public AuditQueryService(IServiceProvider provider) : base(provider)
    {
    }
}

