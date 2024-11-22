using Benutomo;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

[RegisterScoped]
[AutomaticDisposeImpl]
public partial class AuditQueryService : MagicClientSecureService<IAuditQueryService, AUDIT_QUERY>, IAuditQueryService
{
    public AuditQueryService(IServiceProvider provider) : base(provider)
    {
    }
    
    ~AuditQueryService()
    {
        Dispose(false);
    }
}

