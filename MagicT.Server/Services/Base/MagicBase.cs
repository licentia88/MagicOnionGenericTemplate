using AQueryMaker;
using Google.Protobuf.WellKnownTypes;
using MagicOnion;
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using MagicOnion.Server;
using MagicT.Server.Database;
using MagicT.Server.Helpers;
using MagicT.Server.Jwt;
using MagicT.Shared;
using MagicT.Shared.Services.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services.Base;

/// <summary>
/// Base class for magic operations that involve a generic service, model, and database context.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
public class MagicBase<TService, TModel> : MagicBase<TService, TModel, MagicTContext>
    where TService : IGenericService<TService, TModel>, IService<TService>
    where TModel : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicBase{TService, TModel}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public MagicBase(IServiceProvider provider) : base(provider)
    {
        // This constructor initializes the base class with the provided service provider.
        // It allows you to perform operations involving the generic service, model, and database context.
    }
}


/// <summary>
///     Base class for magic operations that involve a generic service, model, and database context.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public class MagicBase<TService, TModel, TContext> : ServiceBase<TService>, IGenericService<TService, TModel>
    where TService : IGenericService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{
    // Dictionary that maps connection names to functions that create SqlQueryFactory instances.
    private readonly IDictionary<string, Func<SqlQueryFactory>> ConnectionFactory;
    
    // A property (commented out) representing a publisher for operations on TModel.
    //public IPublisher<Operation, TModel> Publisher { get; set; }


    // The database context instance used for database operations.
    protected TContext Db;

    // A property for accessing an instance of MemoryDatabase.
    public MemoryDatabase MemoryDatabase { get; set; }


    /// <summary>
    ///     Retrieves the database connection based on the specified connection name.
    /// </summary>
    /// <param name="connectionName">The name of the connection.</param>
    /// <returns>An instance of SqlQueryFactory.</returns>
    protected SqlQueryFactory GetDatabase(string connectionName) => ConnectionFactory[connectionName]?.Invoke();


    /// <summary>
    /// Initializes a new instance of the <see cref="MagicBase{TService, TModel, TContext}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public MagicBase(IServiceProvider provider)
    {
        // Initialize the MemoryDatabase property with the instance retrieved from the service provider.
        MemoryDatabase = provider.GetService<MemoryDatabase>();

        // Set the default serializer provider for MagicOnion to MemoryPackMagicOnionSerializerProvider.
        MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;

        // Initialize the Db field with the instance of the database context retrieved from the service provider.
        Db = provider.GetService<TContext>();

        // Initialize the MagicTTokenService property with the instance retrieved from the service provider.
        MagicTTokenService = provider.GetService<MagicTTokenService>();

        // Initialize the ConnectionFactory field with the instance of the dictionary retrieved from the service provider.
        ConnectionFactory = provider.GetService<IDictionary<string, Func<SqlQueryFactory>>>();    
    }


    /// <summary>
    ///     Gets or sets the instance of FastJwtTokenService.
    /// </summary>
    [Inject]
    public MagicTTokenService MagicTTokenService { get; set; }

    /// <summary>
    ///     Creates a new instance of the specified model.
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
    ///     Finds a list of entities of type TModel that are associated with a parent entity based on a foreign key.
    /// </summary>
    /// <param name="parentId">The identifier of the parent entity.</param>
    /// <param name="foreignKey">The foreign key used to associate the entities with the parent entity.</param>
    /// <returns>
    ///     A <see cref="UnaryResult{List{TModel}}" /> representing the result of the operation, containing a list of
    ///     entities.
    /// </returns>
    public virtual UnaryResult<List<TModel>> FindByParent(string parentId, string foreignKey)
    {
        return TaskHandler.ExecuteAsyncWithoutResponse(async () =>
            await Db.Set<TModel>().FromSql($"SELECT * FROM {typeof(TModel).Name} WHERE {foreignKey} = '{parentId}' ")
                .AsNoTracking().ToListAsync());
    }

    /// <summary>
    ///     Updates the specified model.
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
    ///     Deletes the specified model.
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
    ///     Retrieves all models.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    public virtual UnaryResult<List<TModel>> ReadAll()
    {
        return TaskHandler.ExecuteAsyncWithoutResponse(async () =>
        {
            return await Db.Set<TModel>().AsNoTracking().ToListAsync();
        });
    }

    /// <summary>
    /// Streams all models in batches.
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>A <see cref="ServerStreamingResult{List{TModel}}"/> representing the streamed data.</returns>
    public async Task<ServerStreamingResult<List<TModel>>> StreamReadAll(int batchSize)
    {
        // Get the server streaming context for the list of TModel.
        var stream = GetServerStreamingContext<List<TModel>>();

        // Iterate through the asynchronously fetched data in batches.
        await foreach (var data in FetchStreamAsync(batchSize))
            await stream.WriteAsync(data);

        // Return the result of the streaming context.
        return stream.Result();
    }

   


    /// <summary>
    /// Asynchronously fetches and yields data in batches.
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>An asynchronous enumerable of batches of <typeparamref name="TModel"/>.</returns>
    private async IAsyncEnumerable<List<TModel>> FetchStreamAsync(int batchSize = 10)
    {
        // Get the total count of entities.
        var count = await Db.Set<TModel>().AsNoTracking().CountAsync().ConfigureAwait(false);

        // Calculate the number of batches required.
        var batches = (int) Math.Ceiling((double) count / batchSize);

        for (var i = 0; i < batches; i++)
        {
            var skip = i * batchSize;
            var take = Math.Min(batchSize, count - skip);

            // Fetch a batch of entities asynchronously.
            var entities = await Db.Set<TModel>().AsNoTracking().Skip(skip).Take(take).ToListAsync()
                .ConfigureAwait(false);

            //Yield the batch of entities.
            yield return entities;
        }
    }

    
}