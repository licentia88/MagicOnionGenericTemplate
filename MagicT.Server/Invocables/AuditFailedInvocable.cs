using Coravel.Invocable;
using MagicT.Server.Payloads;

namespace MagicT.Server.Invocables;

/// <summary>
/// Invocable class to handle failed audit operations.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public class AuditFailedInvocable<TContext> : IInvocable, IInvocableWithPayload<AuditFailedPayload>
    where TContext : MagicTContext
{
    /// <summary>
    /// Gets or sets the payload containing the audit query.
    /// </summary>
    public AuditFailedPayload Payload { get; set; }

    private readonly TContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditFailedInvocable{DatabaseContext}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AuditFailedInvocable(TContext context)
    {
        _dbContext = context;
    }

    /// <summary>
    /// Invokes the audit failed operation by adding the audit query to the database and saving changes.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Invoke()
    {
        _dbContext.AUDIT_BASE.Add(Payload.AuditQuery);

        await _dbContext.SaveChangesAsync();
    }
}