using Coravel.Invocable;
using MagicT.Server.Payloads;
using System.Threading;
using Benutomo;

namespace MagicT.Server.Invocables;

/// <summary>
/// Invocable class to handle audit record operations.
/// </summary>
/// <typeparam name="DbContext">The type of the database context.</typeparam>
[AutomaticDisposeImpl]
public partial class AuditRecordsInvocable<DbContext> : IInvocable, IInvocableWithPayload<AuditRecordPayload>, ICancellableInvocable ,IDisposable
    where DbContext : MagicTContext
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
    private readonly DbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditRecordsInvocable{DbContext}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AuditRecordsInvocable(DbContext context)
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