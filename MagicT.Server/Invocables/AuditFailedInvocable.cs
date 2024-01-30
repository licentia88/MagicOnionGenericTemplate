using Coravel.Invocable;
using MagicT.Server.Database;
using MagicT.Server.Payloads;

namespace MagicT.Server.Invocables;

 public partial class AuditFailedInvocable<DbContext> : IInvocable, IInvocableWithPayload<AuditFailedPayload>
    where DbContext : MagicTContext
 {
    public AuditFailedPayload Payload { get; set; }

    private readonly DbContext _dbContext;

    public AuditFailedInvocable(DbContext context)
    {
        _dbContext = context;
    }

    public async Task Invoke()
    {
        _dbContext.AUDIT_BASE.Add(Payload.AuditQuery);

        await _dbContext.SaveChangesAsync();
    }
}

