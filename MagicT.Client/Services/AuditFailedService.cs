using Benutomo;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

[RegisterScoped]
[AutomaticDisposeImpl]
public partial class AuditFailedService : MagicClientSecureService<IAuditFailedService, AUDIT_FAILED>, IAuditFailedService
{
    public AuditFailedService(IServiceProvider provider) : base(provider)
    {
    }
    
    ~AuditFailedService()
    {
        Dispose(false);
    }
}

