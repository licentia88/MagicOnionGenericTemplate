using AQueryMaker;
using AQueryMaker.MSSql;
using MagicOnion;
using MagicOnion.Server;
using MagicT.Server.Invocables;
using MagicT.Server.ZoneTree;
using MagicT.Shared.Extensions;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MagicT.Server.Services.Base;

/* *********** IMPORTANT NOTE: The reason why this class inherits from ServerServiceBase 
 * which inherits ServiceBase is because I need to get streaming context from the service 
*/

/// <summary>
/// Base class for database services providing common database operations.
/// </summary>
/// <typeparam name="TService">The service interface.</typeparam>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TContext">The database context type.</typeparam>
public class DatabaseService<TService, TModel, TContext> :  ServerServiceBase<TService,TModel,TContext>, IMagicService<TService, TModel>
    where TContext : DbContext 
    where TModel : class
    where TService : IMagicService<TService, TModel>, IService<TService>
{
    // The database context instance used for database operations.
    public TContext Database { get; set; }

    // Dictionary that maps connection names to functions that create SqlQueryFactory instances.
    private readonly IDictionary<string, Func<SqlQueryFactory>> ConnectionFactory;

    /// <summary>
    /// ZoneTree Database Manager
    /// </summary>
    public ZoneDbManager ZoneDbManager { get; set; }

    /// <summary>
    ///     Retrieves the database connection based on the specified connection name.
    /// </summary>
    /// <returns>An instance of SqlQueryFactory.</returns>
    public SqlQueryFactory GetDatabase() => new(new SqlServerManager(Database.Database.GetDbConnection()));


    public IDbContextTransaction Transaction => Database.Database.BeginTransaction();

    public DatabaseService(IServiceProvider provider):base(provider)
    {
        ZoneDbManager = provider.GetService<ZoneDbManager>();

        // Initialize the Db field with the instance of the database context retrieved from the service provider.
        Database = provider.GetService<TContext>();

        // Initialize the ConnectionFactory field with the instance of the dictionary retrieved from the service provider.
        ConnectionFactory = provider.GetService<IDictionary<string, Func<SqlQueryFactory>>>();

    }


    /// <summary>
    ///     Creates a new instance of the specified model.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A unary result containing the created model.</returns>
    public virtual async UnaryResult<TModel> CreateAsync(TModel model)
    {
        return await ExecuteWithoutResponseAsync(async () =>
        {
            Database.Set<TModel>().Add(model);
            await Database.SaveChangesAsync();

            return model;
        },Transaction);
    }

    /// <summary>
    ///     Retrieves all models.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    public virtual UnaryResult<List<TModel>> ReadAsync()
    {
        return ExecuteWithoutResponseAsync(async () => await Database.Set<TModel>().AsNoTracking().ToListAsync());
    }

    /// <summary>
    ///     Updates the specified model.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A unary result containing the updated model.</returns>
    public virtual UnaryResult<TModel> UpdateAsync(TModel model)
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            Database.Set<TModel>().Update(model);
            await Database.SaveChangesAsync();
            return model;
        },Transaction);
    }

    /// <summary>
    ///     Deletes the specified model.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A unary result containing the deleted model.</returns>
    public virtual UnaryResult<TModel> DeleteAsync(TModel model)
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            Database.Set<TModel>().Remove(model);
            await Database.SaveChangesAsync();
            return model;
        },Transaction);
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
    public virtual UnaryResult<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return ExecuteWithoutResponseAsync(async () =>
            await Database.Set<TModel>().FromSqlRaw($"SELECT * FROM {typeof(TModel).Name} WHERE {foreignKey} = '{parentId}' ")
                .AsNoTracking().ToListAsync());
    }

    public virtual UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            var dictionary = parameters.UnPickleFromBytes<KeyValuePair<string, object>[]>();

            var whereStatement = string.Join(" AND ", dictionary.Select(x => $" {x.Key} = @{x.Key}").ToList());

            var result = await GetDatabase().QueryAsync($"SELECT * FROM {typeof(TModel).Name} WHERE {whereStatement}", dictionary);

            return result.Adapt<List<TModel>>();

        });

    }

    /// <summary>
    /// Streams all models in batches.
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>A <see cref="ServerStreamingResult{List{TModel}}"/> representing the streamed data.</returns>
    public async Task<ServerStreamingResult<List<TModel>>> StreamReadAllAsync(int batchSize)
    {
        // Get the server streaming context for the list of TModel.
        var stream = GetServerStreamingContext<List<TModel>>();

        // Iterate through the asynchronously fetched data in batches.
        await foreach (var data in FetchStreamAsync(batchSize))
            await stream.WriteAsync(data);

        // Return the result of the streaming context.
        return stream.Result();
    }

    public async UnaryResult<EncryptedData<TModel>> CreateEncrypted(EncryptedData<TModel> encryptedData)
    {
        var sharedKey = GetSharedKey(Context);

        var decryptedData = CryptoHelper.DecryptData(encryptedData, sharedKey);

        var response = await CreateAsync(decryptedData);

        var cryptedData = CryptoHelper.EncryptData(response, sharedKey);

        return cryptedData;
    }

    public async UnaryResult<EncryptedData<List<TModel>>> ReadEncryptedAsync()
    {
        var sharedKey = GetSharedKey(Context);

        var response = await ReadAsync();
                    

        return CryptoHelper.EncryptData(response, sharedKey);
    }

    public async UnaryResult<EncryptedData<TModel>> UpdateEncrypted(EncryptedData<TModel> encryptedData)
    {
        byte[] _sharedSecret = null;

        var decryptedData = CryptoHelper.DecryptData(encryptedData, _sharedSecret);

        var response = await UpdateAsync(decryptedData);

        return CryptoHelper.EncryptData(response, _sharedSecret);
    }

    public async UnaryResult<EncryptedData<TModel>> DeleteEncryptedAsync(EncryptedData<TModel> encryptedData)
    {
        var sharedKey = GetSharedKey(Context);

        var decryptedData = CryptoHelper.DecryptData(encryptedData, sharedKey);

        var response = await DeleteAsync(decryptedData);

        return CryptoHelper.EncryptData(response, sharedKey);
    }

    public async UnaryResult<EncryptedData<List<TModel>>> FindByParentEncryptedAsync(EncryptedData<string> parentId, EncryptedData<string> foreignKey)
    {

        var sharedKey = GetSharedKey(Context);

        var respnseData = await Database.Set<TModel>()
                    .FromSqlRaw($"SELECT * FROM {typeof(TModel).Name} WHERE {foreignKey} = '{parentId}' ")
                    .AsNoTracking().ToListAsync();

        return CryptoHelper.EncryptData(respnseData, sharedKey);
    }

    public UnaryResult<EncryptedData<List<TModel>>> FindByParametersEncryptedAsync(EncryptedData<byte[]> parameterBytes)
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            var connection = GetDatabase();

            var sharedKey = GetSharedKey(Context);

            var decryptedBytes = CryptoHelper.DecryptData(parameterBytes, sharedKey);

            var dictionary = decryptedBytes.UnPickleFromBytes<KeyValuePair<string, object>[]>();

            var whereStatement = string.Join(" AND ", dictionary.Select(x => $" {x.Key} = @{x.Key}").ToList());

            var result = await connection.QueryAsync($"SELECT * FROM {typeof(TModel).Name} WHERE {whereStatement}", dictionary);

            var returnData = result.Adapt<List<TModel>>();

            return CryptoHelper.EncryptData(returnData, sharedKey);

        });
    }

    public async Task<ServerStreamingResult<EncryptedData<List<TModel>>>> StreamReadAllEncyptedAsync(int batchSize)
    {
        var sharedKey = GetSharedKey(Context);

        // Get the server streaming context for the list of TModel.
        var stream = GetServerStreamingContext<EncryptedData<List<TModel>>>();

        // Iterate through the asynchronously fetched data in batches.
        await foreach (var data in FetchStreamAsync(batchSize))
        {
            var responseData = CryptoHelper.EncryptData(data, sharedKey);
            await stream.WriteAsync(responseData);
        }

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
        var count = await Database.Set<TModel>().AsNoTracking().CountAsync().ConfigureAwait(false);

        // Calculate the number of batches required.
        var batches = (int)Math.Ceiling((double)count / batchSize);

        for (var i = 0; i < batches; i++)
        {
            var skip = i * batchSize;
            var take = Math.Min(batchSize, count - skip);

            // Fetch a batch of entities asynchronously.
            var entities = await Database.Set<TModel>().AsNoTracking().Skip(skip).Take(take).ToListAsync()
                .ConfigureAwait(false);

            //Yield the batch of entities.
            yield return entities;
        }
    }

    #region Helper Methods

    /// <summary>
    /// Gets shared key from service context
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected byte[] GetSharedKey(ServiceContext context) => ZoneDbManager.UsersZoneDb
                                                        .FindBy(x => x.UserId == Token(context).Id)
                                                        .FirstOrDefault().Value.SharedKey;
    //protected int GetCurrentUser(ServiceContext context) => Token(context).Id;

    /// <summary>
    /// Adds failed transaction to queue
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    internal Guid AddFailedTransaction(TModel data) => _queue.QueueInvocableWithPayload<FailedTransactionsInvocable<TContext, TModel>, TModel>(data);
    #endregion


}
