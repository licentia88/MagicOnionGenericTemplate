using Coravel.Invocable;
using MagicT.Server.Payloads;
using Benutomo;

namespace MagicT.Server.Invocables;

/// <summary>
/// Invocable class to handle audit record operations.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
// [RegisterTransient(typeof(IInvocable))]
[AutomaticDisposeImpl]
public partial class AuditRecordsInvocable<TContext> : IInvocable, IInvocableWithPayload<AuditRecordPayload>, ICancellableInvocable ,IDisposable,IAsyncDisposable
    where TContext : MagicTContext
{
    /// <summary>
    /// Gets or sets the payload containing the audit records.
    /// </summary>
    [EnableAutomaticDispose]
    public AuditRecordPayload Payload { get; set; }

    /// <summary>
    /// Gets or sets the cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; set; }

    [EnableAutomaticDispose]
    private readonly TContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditRecordsInvocable{DbContext}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AuditRecordsInvocable(TContext context)
    {
        _dbContext = context;
    }

    ~AuditRecordsInvocable()
    {
        Dispose(false);
    }
    
    /// <summary>
    /// Invokes the audit record operation by creating audit records and saving them to the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Invoke()
    {
        Payload.CreateAuditRecord();

        _dbContext.AUDIT_BASE.AddRange(Payload.AuditRecords);

        await _dbContext.SaveChangesAsync(CancellationToken);
    }
}