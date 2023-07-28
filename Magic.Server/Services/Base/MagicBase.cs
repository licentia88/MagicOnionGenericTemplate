using AQueryMaker;
using Magic.Server.Database;
using Magic.Server.Helpers;
using Magic.Server.Jwt;
using Magic.Shared.Services.Base;
using MagicOnion;
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using MagicOnion.Server;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Magic.Server.Services.Base;

public class MagicBase<TService, TModel> : MagicBase<TService, TModel, DummyContext>
   where TService : IGenericService<TService, TModel>, IService<TService>
   where TModel : class
{
    public MagicBase(IServiceProvider provider) : base(provider)
    {

    }


}

/// <summary>
/// Base class for magic operations that involve a generic service, model, and database context.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public class MagicBase<TService, TModel, TContext> : ServiceBase<TService>, IGenericService<TService, TModel>
    where TService : IGenericService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{
    //public IPublisher<Operation, TModel> Publisher { get; set; }

    protected TContext Db;

    private readonly IDictionary<string, Func<SqlQueryFactory>> ConnectionFactory;

    /// <summary>
    /// Gets or sets the instance of FastJwtTokenService.
    /// </summary>
    [Inject]
    public FastJwtTokenService FastJwtTokenService { get; set; }

    /// <summary>
    /// Retrieves the database connection based on the specified connection name.
    /// </summary>
    /// <param name="connectionName">The name of the connection.</param>
    /// <returns>An instance of SqlQueryFactory.</returns>
    protected SqlQueryFactory GetDatabase(string connectionName) => ConnectionFactory[connectionName]?.Invoke();

    public MagicBase(IServiceProvider provider)
    {
        // MemoryDatabase = provider.GetService<MemoryContext>();
        MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;

        Db = provider.GetService<TContext>();
        FastJwtTokenService = provider.GetService<FastJwtTokenService>();
        ConnectionFactory = provider.GetService<IDictionary<string, Func<SqlQueryFactory>>>();
    }

    /// <summary>
    /// Creates a new instance of the specified model.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A unary result containing the created model.</returns>
    public virtual async UnaryResult<TModel> Create(TModel model)
    {
        return await TaskHandler.ExecuteAsyncWithoutResponse(async () =>
        {
            Db.Set<TModel>().Add(model);
            await Db.SaveChangesAsync();
            return model;
        });
    }



    /// <summary>
    /// Finds a list of entities of type TModel that are associated with a parent entity based on a foreign key.
    /// </summary>
    /// <param name="parentId">The identifier of the parent entity.</param>
    /// <param name="foreignKey">The foreign key used to associate the entities with the parent entity.</param>
    /// <returns>A <see cref="UnaryResult{List{TModel}}"/> representing the result of the operation, containing a list of entities.</returns>
    public virtual UnaryResult<List<TModel>> FindByParent(string parentId, string foreignKey)
    {
        return TaskHandler.ExecuteAsyncWithoutResponse(async () =>
        {
            return await Db.Set<TModel>().FromSqlRaw($"SELECT * FROM {typeof(TModel).Name} WHERE {foreignKey} = '{parentId}' ").AsNoTracking().ToListAsync();
        });
    }

    /// <summary>
    /// Updates the specified model.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A unary result containing the updated model.</returns>
    public virtual UnaryResult<TModel> Update(TModel model)
    {
        return TaskHandler.ExecuteAsyncWithoutResponse(async () =>
        {
            Db.Set<TModel>().Update(model);
            await Db.SaveChangesAsync();
            return model;
        });
    }

    /// <summary>
    /// Deletes the specified model.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A unary result containing the deleted model.</returns>
    public virtual UnaryResult<TModel> Delete(TModel model)
    {
        return TaskHandler.ExecuteAsyncWithoutResponse(async () =>
        {
            Db.Set<TModel>().Remove(model);
            await Db.SaveChangesAsync();
            return model;
        });
    }

    /// <summary>
    /// Retrieves all models.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    public virtual UnaryResult<List<TModel>> ReadAll()
    {
        return TaskHandler.ExecuteAsyncWithoutResponse(async () =>
        {
            return await Db.Set<TModel>().AsNoTracking().ToListAsync();
        });
    }

    public async Task<ServerStreamingResult<List<TModel>>> StreamReadAll(int batchSize)
    {
        var stream = GetServerStreamingContext<List<TModel>>();

        await foreach (var data in FetchStreamAsync(batchSize))
            await stream.WriteAsync(data);

        return stream.Result();
    }



    private async IAsyncEnumerable<List<TModel>> FetchStreamAsync(int batchSize = 10)
    {
        var count = await Db.Set<TModel>().AsNoTracking().CountAsync().ConfigureAwait(false);
        var batches = (int)Math.Ceiling((double)count / batchSize);

        for (var i = 0; i < batches; i++)
        {
            var skip = i * batchSize;
            var take = Math.Min(batchSize, count - skip);
            var entities = await Db.Set<TModel>().AsNoTracking().Skip(skip).Take(take).ToListAsync().ConfigureAwait(false);
            yield return entities;
        }

    }

    // [Obsolete(message:"This method is meant to be used from the client side, do not call")]
    //public TService SetToken(byte[] token)
    //{
    //    throw new NotImplementedException();
    //}
}

