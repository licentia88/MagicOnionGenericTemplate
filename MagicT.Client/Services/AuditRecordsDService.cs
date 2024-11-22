using Benutomo;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

[RegisterScoped]
[AutomaticDisposeImpl]
public partial class AuditRecordsDService : MagicClientSecureService<IAuditRecordsDService, AUDIT_RECORDS_D>, IAuditRecordsDService
{
    public AuditRecordsDService(IServiceProvider provider) : base(provider)
    {
    }
    
    ~AuditRecordsDService()
    {
        Dispose(false);
    }
}

