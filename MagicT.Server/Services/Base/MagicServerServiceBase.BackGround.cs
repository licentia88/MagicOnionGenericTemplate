using MagicT.Server.BackgroundTasks;
using MagicT.Server.Database;
using MagicT.Shared.Models;

namespace MagicT.Server.Services.Base;

public partial class MagicServerServiceBase<TService, TModel, TContext>
{
    public IBackGroundTaskQueue BackGroundTaskQueue { get; set; }

    Func<FAILED_TRANSACTIONS_LOG> _workItem { get; set; }

 
    async Task BuildBackGroundWorkForFailedTransactions(CancellationToken cancellationToken)
    {
        var log = _workItem.Invoke();

        using var db = BackGroundTaskQueue.Provider.GetService<MagicTContext>();

        await db.FAILED_TRANSACTIONS_LOG.AddAsync(log, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}

