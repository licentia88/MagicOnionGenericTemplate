using Benutomo;
using Coravel.Invocable;
using MagicT.Server.Payloads;

namespace MagicT.Server.Invocables;

/// <summary>
/// Invocable class to handle audit query operations.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
// [RegisterTransient(typeof(IInvocable))]
[AutomaticDisposeImpl] 
public partial class AuditQueryInvocable<TContext> : IInvocable, IInvocableWithPayload<AuditQueryPayload>,IDisposable,IAsyncDisposable
    where TContext : MagicTContext
{
    /// <summary>
    /// Gets or sets the payload containing the audit query.
    /// </summary>
    [EnableAutomaticDispose]
    public AuditQueryPayload Payload { get; set; }

    [EnableAutomaticDispose]
    private readonly TContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditQueryInvocable{TContext}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AuditQueryInvocable(TContext context)
    {
        _dbContext = context;
    }

    ~AuditQueryInvocable()
    {
        Dispose(false);
    }
    /// <summary>
    /// Invokes the audit query operation by adding the audit query to the database and saving changes.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Invoke()
    {
        _dbContext.AUDIT_BASE.Add(Payload.AuditQuery);

        await _dbContext.SaveChangesAsync();
    }
}