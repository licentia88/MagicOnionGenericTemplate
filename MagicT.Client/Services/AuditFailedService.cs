using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

[RegisterScoped]
public sealed class AuditFailedService : MagicClientSecureService<IAuditFailedService, AUDIT_FAILED>, IAuditFailedService
{
    public AuditFailedService(IServiceProvider provider) : base(provider)
    {
    }
}

