namespace MagicT.Server.Invocables;

public class AuditRecordsInvocable<DbContext> : global::Coravel.Invocable.IInvocable, global::Coravel.Invocable.IInvocableWithPayload<global::MagicT.Server.Payloads.AuditRecordPayload>, global::Coravel.Invocable.ICancellableInvocable where DbContext:MagicTContext
{
    public global::MagicT.Server.Payloads.AuditRecordPayload Payload { get; set; }

    public global::System.Threading.CancellationToken CancellationToken { get; set; }

    private readonly DbContext _dbContext;

    public AuditRecordsInvocable(DbContext context)
    {
        _dbContext = context;
    }

    public async global::System.Threading.Tasks.Task Invoke()
    {
        Payload.CreateAuditRecord();

        _dbContext.AUDIT_BASE.AddRange(Payload.AUDIT_RECORDS);

        try
        {
            await _dbContext.SaveChangesAsync(CancellationToken);
        }
        catch (global::System.Exception ex)
        {

        }
    }
}

