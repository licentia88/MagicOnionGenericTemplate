using AQueryMaker;
using MagicOnion;
using MagicOnion.Server.Hubs;
using MagicT.Server.Database;
using MagicT.Server.Exceptions;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Hubs.Base;
using MagicT.Shared.Models.ServiceModels;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Hubs.Base;

/// <summary>
/// A base class for MagicOnion streaming hubs with CRUD operations and authorization capabilities.
/// </summary>
/// <typeparam name="THub">The type of the streaming hub.</typeparam>
/// <typeparam name="TReceiver">The type of the receiver.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
public partial class MagicHubServerBase<THub, TReceiver, TModel> : MagicHubServerBase<THub, TReceiver, TModel, MagicTContext>
    where THub : IStreamingHub<THub, TReceiver>
    where TReceiver : IMagicReceiver<TModel>
    where TModel : class, new()
{


    /// <summary>
    /// Initializes a new instance of the <see cref="MagicHubServerBase{THub, TReceiver, TModel}"/> class.
    /// </summary>
    /// <param name="provider">The service provider for dependency resolution.</param>
    public MagicHubServerBase(IServiceProvider provider) : base(provider)
    {
    }
}

/// <summary>
/// A base class for MagicOnion streaming hubs with CRUD operations, authorization, and database context capabilities.
/// </summary>
/// <typeparam name="THub">The type of the streaming hub.</typeparam>
/// <typeparam name="TReceiver">The type of the receiver.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public partial class MagicHubServerBase<THub, TReceiver, TModel, TContext> : StreamingHubBase<THub, TReceiver>,
    IMagicHub<THub, TReceiver, TModel>
    where THub : IStreamingHub<THub, TReceiver>
    where TReceiver : IMagicReceiver<TModel>
    where TContext : DbContext
    where TModel : class, new()
{
    private readonly IDictionary<string, Func<SqlQueryFactory>> ConnectionFactory;

    public DbExceptionHandler DbExceptionHandler { get; set; }


    /// <summary>
    /// Retrieves the database connection based on the specified connection name.
    /// </summary>
    /// <param name="connectionName">The name of the connection.</param>
    /// <returns>An instance of <see cref="SqlQueryFactory"/>.</returns>
    protected SqlQueryFactory GetDatabase(string connectionName) => ConnectionFactory[connectionName]?.Invoke();
        

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicHubServerBase{THub, TReceiver, TModel, TContext}"/> class.
    /// </summary>
    /// <param name="provider">The service provider for dependency resolution.</param>
    public MagicHubServerBase(IServiceProvider provider)
    {
        Db = provider.GetService<TContext>();
        DbExceptionHandler = provider.GetService<DbExceptionHandler>();
        MagicTTokenService = provider.GetService<MagicTTokenService>();
        ConnectionFactory = provider.GetService<IDictionary<string, Func<SqlQueryFactory>>>();
        Subscriber = provider.GetService<ISubscriber<string, (Operation, TModel)>>();
    }

    protected TContext Db;
    protected IGroup Room;

    protected List<TModel> Collection { get; set; }

    protected ISubscriber<string, (Operation operation, TModel model)> Subscriber { get; }

    /// <summary>
    /// Gets or sets the instance of <see cref="MagicTTokenService"/>.
    /// </summary>
    [Inject]
    public MagicTTokenService MagicTTokenService { get; set; }

    /// <inheritdoc />
    public async Task ConnectAsync()
    {
        Collection = new List<TModel>();

        Room = await Group.AddAsync(typeof(TModel).Name);

        var disposable = Subscriber.Subscribe(Context.GetClientName(), data =>
        {
            switch (data.operation)
            {
                case Operation.Create:
                    Collection.Add(data.model);
                    Broadcast(Room).OnCreate(data.model);
                    break;
                case Operation.Update:
                {
                    var index = Collection.IndexOf(data.model);
                    Collection[index] = data.model;
                    Broadcast(Room).OnUpdate(data.model);
                    break;
                }
                case Operation.Delete:
                    Collection.Remove(data.model);
                    Broadcast(Room).OnDelete(data.model);
                    break;
            }
        });
    }

    /// <inheritdoc />
    public virtual async Task<RESPONSE_RESULT<TModel>> CreateAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Add(model);
            await Db.SaveChangesAsync();
            Collection.Add(model);
            Broadcast(Room).OnCreate(model);
            return model;
        });
    }

    /// <inheritdoc />
    public virtual async Task<RESPONSE_RESULT<TModel>> DeleteAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Remove(model);
            await Db.SaveChangesAsync();
            Collection.Remove(model);
            Broadcast(Room).OnDelete(model);
            return model;
        });
    }

    /// <inheritdoc />
    public virtual async Task<RESPONSE_RESULT<List<TModel>>> ReadAsync()
    {
        return await ExecuteAsync(async () =>
        {
            var data = await Db.Set<TModel>().AsNoTracking().ToListAsync();
            var uniqueData = data.Except(Collection).ToList();
            if (uniqueData.Count != 0)
            {
                BroadcastToSelf(Room).OnRead(uniqueData);
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
            Broadcast(Room).OnStreamRead(uniqueData);
            Collection.AddRange(uniqueData);
        }
    }

    /// <inheritdoc />
    public virtual async Task<RESPONSE_RESULT<TModel>> UpdateAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            var existing = Db.Entry(model).OriginalValues.ToModel<TModel>();
            Db.Set<TModel>().Attach(model);
            Db.Set<TModel>().Update(model);
            await Db.SaveChangesAsync();
            Collection.Replace(existing, model);
            Broadcast(Room).OnUpdate(model);
            return model;
        });
    }

    /// <inheritdoc />
    public virtual async Task CollectionChanged()
    {
        var newCollection = await Db.Set<TModel>().AsNoTracking().ToListAsync();
        Collection.Clear();
        Collection.AddRange(newCollection);
        Broadcast(Room).OnCollectionChanged(Collection);
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

}