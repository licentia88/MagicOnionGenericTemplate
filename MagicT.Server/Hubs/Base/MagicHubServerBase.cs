using Benutomo;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server.Hubs;
using MagicT.Server.Exceptions;
using MagicT.Server.Jwt;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Hubs.Base;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MagicT.Server.Hubs.Base;

/// <summary>
/// A base class for MagicOnion streaming hubs with CRUD operations, authorization, and database context capabilities.
/// </summary>
/// <typeparam name="THub">The type of the streaming hub.</typeparam>
/// <typeparam name="TReceiver">The type of the receiver.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
[AutomaticDisposeImpl]
public abstract partial class MagicHubServerBase<THub, TReceiver, TModel, TContext> : StreamingHubBase<THub, TReceiver>,
    IMagicHub<THub, TReceiver, TModel>, IDisposable,IAsyncDisposable
    where THub : IStreamingHub<THub, TReceiver>
    where TReceiver : IMagicReceiver<TModel>
    where TContext : DbContext
    where TModel : class, new()
{
    [EnableAutomaticDispose]
    protected IDbContextTransaction Transaction;

    public DbExceptionHandler DbExceptionHandler { get; set; }

    [EnableAutomaticDispose]
    protected TContext Db;

    protected IGroup Room;

    public List<TModel> Collection { get; set; }

    protected ISubscriber<Guid, (Operation operation, TModel model)> Subscriber { get; }

 
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicHubServerBase{THub, TReceiver, TModel, TContext}"/> class.
    /// </summary>
    /// <param name="provider">The service provider for dependency resolution.</param>
    public MagicHubServerBase(IServiceProvider provider)
    {
        Db = provider.GetService<TContext>();
        DbExceptionHandler = provider.GetService<DbExceptionHandler>();
        MagicTTokenService = provider.GetService<MagicTTokenService>();
        Subscriber = provider.GetService<ISubscriber<Guid, (Operation, TModel)>>();
    }

  

    /// <summary>
    /// Gets or sets the instance of <see cref="MagicTTokenService"/>.
    /// </summary>
    [Inject]
    [EnableAutomaticDispose]
    public MagicTTokenService MagicTTokenService { get; set; }

    public IInMemoryStorage<TModel> storage { get; set; }
    /// <inheritdoc />
    public virtual async Task<Guid> ConnectAsync()
    {
        Collection = new List<TModel>();

        Room = await Group.AddAsync(typeof(TModel).Name);

        //USAGE DESCRIPTION:
        //Servicelerden  hub tetiklenme istendiginde
        //ConnectionId ile publish yapilarak burasi tetiklenebilir
        //ConnectionId Client tarafinda connect methodunda geri donus yapar
        //var disposable = Subscriber.Subscribe(ConnectionId, data =>
        //{
        //    switch (data.operation)
        //    {
        //        case Operation.Create:
        //            Collection.Add(data.model);
        //            BroadcastTo(Room,ConnectionId).OnCreate(data.model);
        //            break;
        //        case Operation.Update:
        //        {
        //            var index = Collection.IndexOf(data.model);
        //            Collection[index] = data.model;
        //            BroadcastTo(Room, ConnectionId).OnUpdate(data.model);
        //            break;
        //        }
        //        case Operation.Delete:
        //            Collection.Remove(data.model);
        //            BroadcastTo(Room, ConnectionId).OnDelete(data.model);
        //            break;
        //    }
        //});

        return ConnectionId;
    }

    /// <inheritdoc />
    public virtual async Task CreateAsync(TModel model)
    {
        await  ExecuteAsync(async () =>
        {
            await Db.Set<TModel>().AddAsync(model);

            await Db.SaveChangesAsync();
            Db.ChangeTracker.Clear();
            Collection.Add(model);

            BroadcastTo(Room,ConnectionId).OnCreate(model);
            //Broadcast(Room).OnCreate(model);

          
            return model;
        });
       
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(TModel model)
    { 
        await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Remove(model);

            await Db.SaveChangesAsync();
            Db.ChangeTracker.Clear();
            Collection.Remove(model);

            //Broadcast(Room).OnDelete(model);

            BroadcastTo(Room, ConnectionId).OnDelete(model);

            
            return model;
        });
    }

    /// <inheritdoc />
    public virtual async Task ReadAsync()
    { 
        await ExecuteAsync(async () =>
        {
            var data = await Db.Set<TModel>().AsNoTracking().ToListAsync();

            var uniqueData = data.Except(Collection).ToList();

            if (uniqueData.Count != 0)
            {
                BroadcastTo(Room, ConnectionId).OnRead(uniqueData);

                //BroadcastToSelf(Room).OnRead(uniqueData);
                Collection.AddRange(uniqueData);
            }
            return Collection;
        });
    }

    /// <inheritdoc />
    public async Task StreamReadAsync(int batchSize)
    {
        await foreach (var data in FetchStreamAsync(batchSize))
        {
            var uniqueData = data.Except(Collection).ToList();
            if (uniqueData.Count == 0) continue;

            BroadcastTo(Room, ConnectionId).OnStreamRead(uniqueData);

            //Broadcast(Room).OnStreamRead(uniqueData);
            Collection.AddRange(uniqueData);
        }
    }

    /// <inheritdoc />
    public virtual async Task UpdateAsync(TModel model)
    {
         await ExecuteAsync(async () =>
        {
            var existing = Db.Entry(model).OriginalValues.ToModel<TModel>();
            Db.Set<TModel>().Attach(model);
            Db.Set<TModel>().Update(model);
            await Db.SaveChangesAsync();
            Db.ChangeTracker.Clear();
            Collection.Replace(existing, model);


            BroadcastTo(Room,ConnectionId).OnUpdate(model);

            return model;
        });
    }

    /// <inheritdoc />
    public virtual async Task CollectionChanged()
    {
        var newCollection = await Db.Set<TModel>().AsNoTracking().ToListAsync();

        Collection.Clear();

        Collection.AddRange(newCollection);

        BroadcastTo(Room, ConnectionId).OnCollectionChanged(Collection);
    }


    /// <summary>
    /// Asynchronously fetches the data from the database in batches and yields each batch as a list.
    /// </summary>
    /// <param name="batchSize">The size of each batch to fetch from the database. Default value is 2.</param>
    /// <returns>An asynchronous enumerable that yields lists of <typeparamref name="TModel"/> objects.</returns>
    private async IAsyncEnumerable<List<TModel>> FetchStreamAsync(int batchSize = 2)
    {
        // Get the total count of items in the database table.
        var count = await Db.Set<TModel>().CountAsync().ConfigureAwait(false);

        // Calculate the number of batches based on the total count and batch size.
        var batches = (int)Math.Ceiling((double)count / batchSize);

        // Fetch and yield each batch of data as a list.
        for (var i = 0; i < batches; i++)
        {
            var skip = i * batchSize;
            var take = Math.Min(batchSize, count - skip);

            // Retrieve the data from the database in the current batch range.
            var entities = await Db.Set<TModel>().AsNoTracking().Skip(skip).Take(take).ToListAsync()
                .ConfigureAwait(false);

            // Yield the list of entities in the current batch.
            yield return entities;
        }
    }

    protected virtual async UnaryResult<T> ExecuteAsync<T>(Func<Task<T>> task)
    {
        try
        {
            var result = await task().ConfigureAwait(false);

            if (Transaction is not null)
                await Transaction.CommitAsync();

            return result;
        }
        catch (Exception ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }

    }

    public virtual UnaryResult<T> Execute<T>(Func<T> task)
    {
        try
        {
            var result = task();

            if (Transaction is not null)
                Transaction.Commit();

            return UnaryResult.FromResult(result);
        }
        catch (Exception ex)
        {
            if (Transaction is not null)
                Transaction.Rollback();

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }
    }

    /// <summary>
    ///     Executes an action.
    /// </summary>
    /// <param name="task">The action to execute.</param>
    public virtual void Execute(Action task)
    {
        try
        {
            task();

            if (Transaction is not null)
                Transaction.Commit();
        }
        catch (Exception ex)
        {
            if (Transaction is not null)
                Transaction.Rollback();

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }
    }

    /// <summary>
    /// Executes an asynchronous task without returning a response.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>

    /// <summary>
    ///     Exception Handling
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    private string HandleException(Exception ex)
    {

        return DbExceptionHandler.HandleException(ex);
    }

    protected void BeginTransaction()
    {
        Transaction = Db.Database.BeginTransaction();
    }

    protected async Task BeginTransactionAsync()
    {
        Transaction = await Db.Database.BeginTransactionAsync();
    }
}