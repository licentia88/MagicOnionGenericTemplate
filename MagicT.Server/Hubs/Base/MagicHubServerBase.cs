using AQueryMaker.Extensions;
using Benutomo;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server.Hubs;
using MagicT.Server.Exceptions;
using MagicT.Server.Jwt;
using MagicT.Server.Managers;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Hubs.Base;
using Mapster;
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

    [EnableAutomaticDispose]
    public QueryManager QueryManager { get; set; }

 
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
        QueryManager = provider.GetService<QueryManager>();
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
    public virtual async Task<TModel> CreateAsync(TModel model)
    {
        return await  ExecuteAsync(async () =>
        {
            await Db.Set<TModel>().AddAsync(model);

            await Db.SaveChangesAsync();
            
            Db.ChangeTracker.Clear();
            
            Collection.Add(model);

            BroadcastExceptSelf(Room).OnCreate(model);
          
            return model;
        });
       
    }

    /// <inheritdoc />
    public virtual async Task<TModel> DeleteAsync(TModel model)
    { 
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Remove(model);

            await Db.SaveChangesAsync();
            
            Db.ChangeTracker.Clear();
            
            Collection.Remove(model);

            //Broadcast(Room).OnDelete(model);

            BroadcastExceptSelf(Room).OnDelete(model);

            
            return model;
        });
    }

    /// <summary>
    ///     Finds a list of entities of type TModel that are associated with a parent entity based on a foreign key.
    /// </summary>
    /// <param name="parentId">The identifier of the parent entity.</param>
    /// <param name="foreignKey">The foreign key used to associate the entities with the parent entity.</param>
    /// <returns>
    ///     A <see cref="UnaryResult{ListTModel}" /> representing the result of the operation, containing a list of
    ///     entities.
    /// </returns>
    public virtual Task<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return ExecuteAsync(async () =>
        {
            KeyValuePair<string, object> parameter = new(foreignKey, parentId);
             
            var queryData = QueryManager.BuildQuery<TModel>(parameter);

            var queryResult = await Db.SqlManager().QueryAsync(queryData.query, queryData.parameters);

            return queryResult.Adapt<List<TModel>>();
        });
    }

    public virtual Task<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return ExecuteAsync(async () =>
        {
            var queryData = QueryManager.BuildQuery<TModel>(parameters);

            var result = await Db.SqlManager().QueryAsync(queryData.query, queryData.parameters);

            return result.Adapt<List<TModel>>();

        });
    }

    /// <inheritdoc />
    public virtual async Task ReadAsync()
    { 
        await ExecuteAsync(async () =>
        {
            var data = await Db.Set<TModel>().Where(x=> !Collection.Contains(x)).AsNoTracking().ToListAsync();

            BroadcastTo(Room, ConnectionId).OnRead(data);

            //BroadcastToSelf(Room).OnRead(uniqueData);
            Collection.AddRange(data);

            return Collection;
        });
    }

    /// <inheritdoc />
    public async Task StreamReadAsync(int batchSize)
    {
        await foreach (var data in FetchStreamAsync(batchSize))
        {
            BroadcastTo(Room, ConnectionId).OnStreamRead(data);

            Collection.AddRange(data);
        }
    }

    /// <inheritdoc />
    public virtual async Task<TModel> UpdateAsync(TModel model)
    {
        return  await ExecuteAsync(async () =>
        {
            var existing = Db.Entry(model).OriginalValues.ToModel<TModel>();
            Db.Set<TModel>().Attach(model);
            Db.Set<TModel>().Update(model);
            await Db.SaveChangesAsync();
            
            Db.ChangeTracker.Clear();
            
            Collection.Replace(existing, model);


            BroadcastExceptSelf(Room).OnUpdate(model);

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
        var count = await Db.Set<TModel>().Where(x=> !Collection.Contains(x)).CountAsync().ConfigureAwait(false);

        // Calculate the number of batches based on the total count and batch size.
        var batches = (int)Math.Ceiling((double)count / batchSize);

        // Fetch and yield each batch of data as a list.
        for (var i = 0; i < batches; i++)
        {
            var skip = i * batchSize;
            var take = Math.Min(batchSize, count - skip);

            // Retrieve the data from the database in the current batch range.
            var entities = await Db.Set<TModel>()
                .Where(x => !Collection.Contains(x))
                .AsNoTracking()
                .Skip(skip)
                .Take(take).ToListAsync();

            // Yield the list of entities in the current batch.
            yield return entities;
        }
    }

    protected virtual async Task<T> ExecuteAsync<T>(Func<Task<T>> task)
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

    public virtual Task Execute<T>(Func<T> task)
    {
        try
        {
            var result = task();

            if (Transaction is not null)
                Transaction.Commit();

            return Task.FromResult(result);
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