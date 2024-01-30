using Coravel.Invocable;
using MagicT.Server.Database;
using MagicT.Server.Payloads;

namespace MagicT.Server.Invocables;

public class AuditRecordsInvocable<DbContext> : IInvocable, IInvocableWithPayload<AuditRecordPayload>,ICancellableInvocable where DbContext:MagicTContext
{
    public AuditRecordPayload Payload { get; set; }

    public CancellationToken CancellationToken { get; set; }

    private readonly DbContext _dbContext;

    public AuditRecordsInvocable(DbContext context)
    {
        _dbContext = context;
    }

    public async Task Invoke()
    {
        Payload.CreateAuditRecord();

        _dbContext.AUDIT_BASE.AddRange(Payload.AUDIT_RECORDS);

        await _dbContext.SaveChangesAsync(CancellationToken);
    }
}

