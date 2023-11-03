using Coravel.Invocable;
using MagicT.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Invocables;

public class FailedTransactionsInvocable<TContext,TModel> : IInvocable, IInvocableWithPayload<TModel>
    where TContext : DbContext where TModel:class
{
    public TContext Context { get; set; }

    public TModel Payload { get; set; }

    public FailedTransactionsInvocable(TContext context)
    {
        Context = context;
    }

    public async Task Invoke()
    {
        await Context.Set<TModel>().AddAsync(Payload);
        await Context.SaveChangesAsync();
    }
}

