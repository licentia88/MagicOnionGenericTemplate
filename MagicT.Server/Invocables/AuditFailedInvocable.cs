using MagicT.Server.Database;
using MagicT.Server.Payloads;

namespace MagicT.Server.Invocables;

 public partial class AuditFailedInvocable<DbContext> : global::Coravel.Invocable.IInvocable, global::Coravel.Invocable.IInvocableWithPayload<AuditFailedPayload>
    where DbContext : MagicTContext
 {
    public AuditFailedPayload Payload { get; set; }

    private readonly DbContext _dbContext;

    public AuditFailedInvocable(DbContext context)
    {
        _dbContext = context;
    }

    public async global::System.Threading.Tasks.Task Invoke()
    {
        _dbContext.AUDIT_BASE.Add(Payload.AuditQuery);

        await _dbContext.SaveChangesAsync();
    }
}

