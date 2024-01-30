using Coravel.Invocable;
using MagicT.Server.Database;
using MagicT.Server.Payloads;

namespace MagicT.Server.Invocables;

public class AuditQueryInvocable<TContext> : IInvocable, IInvocableWithPayload<AuditQueryPayload>
    where TContext:MagicTContext
{
    public AuditQueryPayload Payload { get; set; }

    private readonly TContext _dbContext;

    public AuditQueryInvocable(TContext context)
    {
        _dbContext = context;
    }

    public async Task Invoke()
    {
        _dbContext.AUDIT_BASE.Add(Payload.AuditQuery);

        await _dbContext.SaveChangesAsync();
    }


}