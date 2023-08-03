using AQueryMaker;
using MagicOnion;
using MagicOnion.Server.Hubs;
using MagicT.Server.Database;
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

public class MagicHubServerBase<THub, TReceiver, TModel> : MagicHubServerBase<THub, TReceiver, TModel, MagicTContext>
    where THub : IStreamingHub<THub, TReceiver>
    where TReceiver : IMagicReceiver<TModel>
    where TModel : class, new()
{
    public MagicHubServerBase(IServiceProvider provider) : base(provider)
    {
    }
}


public class MagicHubServerBase<THub, TReceiver, TModel, TContext> : StreamingHubBase<THub, TReceiver>, IMagicHub<THub, TReceiver, TModel>
    where THub : IStreamingHub<THub, TReceiver>
    where TReceiver : IMagicReceiver<TModel>
    where TContext : DbContext
    where TModel : class, new()
{
    protected IGroup Room;

    protected List<TModel> Collection { get; set; }

    //protected IInMemoryStorage<List<TModel>> Storage;

    protected ISubscriber<string, (Operation operation, TModel model)> Subscriber { get; }

    protected TContext Db;

    private readonly IDictionary<string, Func<SqlQueryFactory>> ConnectionFactory;

    /// <summary>
    /// Gets or sets the instance of FastJwtTokenService.
    /// </summary>
    [Inject]
    public MagicTTokenService MagicTTokenService { get; set; }

    /// <summary>
    /// Retrieves the database connection based on the specified connection name.
    /// </summary>
    /// <param name="connectionName">The name of the connection.</param>
    /// <returns>An instance of SqlQueryFactory.</returns>
    protected SqlQueryFactory GetDatabase(string connectionName) => ConnectionFactory[connectionName]?.Invoke();
    public MagicHubServerBase(IServiceProvider provider)
    {
        Db = provider.GetService<TContext>();
        MagicTTokenService = provider.GetService<MagicTTokenService>();
        ConnectionFactory = provider.GetService<IDictionary<string, Func<SqlQueryFactory>>>();
        Subscriber = provider.GetService<ISubscriber<string, (Operation, TModel)>>();
        //Collection = provider.GetService<List<TModel>>();
    }

    public async Task ConnectAsync()
    {
        Collection = new List<TModel>();

        Room = await Group.AddAsync(typeof(TModel).Name);
        //(Room, Storage) = await Group.AddAsync(typeof(TModel).Name, Collection);


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

    public virtual async Task<RESPONSE_RESULT<TModel>> CreateAsync(TModel model)
    {
        return await TaskHandler.ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Add(model);

            await Db.SaveChangesAsync();

            Collection.Add(model);

            Broadcast(Room).OnCreate(model);

            return model;
        });
    }

    public virtual async Task<RESPONSE_RESULT<TModel>> DeleteAsync(TModel model)
    {
        return await TaskHandler.ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Remove(model);

            await Db.SaveChangesAsync();

            Collection.Remove(model);

            Broadcast(Room).OnDelete(model);

            return model;
        });
    }


    public virtual async Task<RESPONSE_RESULT<List<TModel>>> ReadAsync()
    {
        return await TaskHandler.ExecuteAsync(async () =>
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

    public virtual async Task<RESPONSE_RESULT<TModel>> UpdateAsync(TModel model)
    {
        return await TaskHandler.ExecuteAsync(async () =>
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

    public virtual async Task CollectionChanged()
    {
        var newCollection = await Db.Set<TModel>().AsNoTracking().ToListAsync();

        Collection.Clear();
        Collection.AddRange(newCollection);

        Broadcast(Room).OnCollectionChanged(Collection);
    }

    private async IAsyncEnumerable<List<TModel>> FetchStreamAsync(int batchSize = 2)
    {
        var count = await Db.Set<TModel>().CountAsync().ConfigureAwait(false);
        var batches = (int)Math.Ceiling((double)count / batchSize);

        for (var i = 0; i < batches; i++)
        {
            var skip = i * batchSize;
            var take = Math.Min(batchSize, count - skip);
            var entities = await Db.Set<TModel>().AsNoTracking().Skip(skip).Take(take).ToListAsync().ConfigureAwait(false);
            yield return entities;
        }
    }
}

