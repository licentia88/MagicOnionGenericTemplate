using AQueryMaker;
using AQueryMaker.MSSql;
using MagicOnion;
using MagicOnion.Server;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;
using MagicT.Server.ZoneTree;
using MagicT.Shared.Extensions;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services.Base;

public abstract partial class MagicServerServiceBase<TService, TModel, TContext>
{
    
    // Dictionary that maps connection names to functions that create SqlQueryFactory instances.
    private readonly IDictionary<string, Func<SqlQueryFactory>> ConnectionFactory;

    // The database context instance used for database operations.
    protected TContext Db;

    // A property for accessing an instance of MemoryDatabaseManager.
    //public MemoryDatabaseManager MemoryDatabaseManager { get; set; }


    public ZoneDbManager ZoneDbManager { get; set; }
    /// <summary>
    ///     Retrieves the database connection based on the specified connection name.
    /// </summary>
    /// <returns>An instance of SqlQueryFactory.</returns>
    protected SqlQueryFactory GetDatabase() =>
        new SqlQueryFactory(new SqlServerManager(Db.Database.GetDbConnection()));


    /// <summary>
    ///     Creates a new instance of the specified model.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A unary result containing the created model.</returns>
    public virtual async UnaryResult<TModel> Create(TModel model)
    {
        return await ExecuteAsyncWithoutResponse(async () =>
        {
            Db.Set<TModel>().Add(model);
            await Db.SaveChangesAsync();
            return model;
        }, nameof(MagicServerServiceBase<TService, TModel>.Create));
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
        return ExecuteAsyncWithoutResponse(async () =>
            await Db.Set<TModel>().FromSqlRaw($"SELECT * FROM {typeof(TModel).Name} WHERE {foreignKey} = '{parentId}' ")
                .AsNoTracking().ToListAsync(),  nameof(MagicServerServiceBase<TService, TModel>.FindByParent));
    }

    /// <summary>
    ///     Updates the specified model.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A unary result containing the updated model.</returns>
    public virtual UnaryResult<TModel> Update(TModel model)
    {
        return ExecuteAsyncWithoutResponse(async () =>
        {
            Db.Set<TModel>().Update(model);
            await Db.SaveChangesAsync();
            return model;
        }, nameof(MagicServerServiceBase<TService, TModel>.Update));
    }

    /// <summary>
    ///     Deletes the specified model.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A unary result containing the deleted model.</returns>
    public virtual UnaryResult<TModel> Delete(TModel model)
    {
        return ExecuteAsyncWithoutResponse(async () =>
        {
            Db.Set<TModel>().Remove(model);
            await Db.SaveChangesAsync();
            return model;
        }, nameof(MagicServerServiceBase<TService, TModel>.Delete));
    }

    /// <summary>
    ///     Retrieves all models.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    public virtual UnaryResult<List<TModel>> Read()
    {
        return ExecuteAsyncWithoutResponse(async () =>
        {
            return await Db.Set<TModel>().AsNoTracking().ToListAsync();
        }, nameof(MagicServerServiceBase<TService,TModel>.Read));
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
        var batches = (int)Math.Ceiling((double)count / batchSize);

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

    public MagicTToken Token(ServiceContext context) => Context.GetItemAs<MagicTToken>(nameof(MagicTToken));

    public byte[] GetSharedKey(ServiceContext context)
            => ZoneDbManager.UsersZoneDb
                            .FindBy(x => x.UserId == Token(context).Id)
                                                                       .FirstOrDefault().Value.SharedKey;


    public async UnaryResult<EncryptedData<TModel>> CreateEncrypted(EncryptedData<TModel> encryptedData)
    {
        var sharedKey = GetSharedKey(Context);

        var decryptedData = CryptoHelper.DecryptData(encryptedData, sharedKey);

        var response = await Create(decryptedData);

        var  cryptedData = CryptoHelper.EncryptData(response, sharedKey);

        return cryptedData;
    }

    public async UnaryResult<EncryptedData<List<TModel>>> ReadEncrypted()
    {
        var sharedKey = GetSharedKey(Context);

        var response = await Read();

        return CryptoHelper.EncryptData(response, sharedKey);
    }

    public async UnaryResult<EncryptedData<TModel>> UpdateEncrypted(EncryptedData<TModel> encryptedData)
    {
        byte[] _sharedSecret = null;

        var decryptedData = CryptoHelper.DecryptData(encryptedData, _sharedSecret);

        var response = await Update(decryptedData);

        return CryptoHelper.EncryptData(response, _sharedSecret);
    }

    public async UnaryResult<EncryptedData<TModel>> DeleteEncrypted(EncryptedData<TModel> encryptedData)
    {
        var sharedKey = GetSharedKey(Context);

        var decryptedData = CryptoHelper.DecryptData(encryptedData, sharedKey);

        var response = await Delete(decryptedData);

        return CryptoHelper.EncryptData(response, sharedKey);
    }

    public async UnaryResult<EncryptedData<List<TModel>>> FindByParentEncryptedAsync(EncryptedData<string> parentId, EncryptedData<string> foreignKey)
    {

        var sharedKey = GetSharedKey(Context);

        var respnseData = await Db.Set<TModel>()
                    .FromSqlRaw($"SELECT * FROM {typeof(TModel).Name} WHERE {foreignKey} = '{parentId}' ")
                    .AsNoTracking().ToListAsync();

        return CryptoHelper.EncryptData(respnseData, sharedKey);
    }
   
        
    

    public async Task<ServerStreamingResult<EncryptedData<List<TModel>>>> StreamReadAllEncypted(int batchSize)
    {
        var sharedKey = GetSharedKey(Context);

        // Get the server streaming context for the list of TModel.
        var stream = GetServerStreamingContext<EncryptedData<List<TModel>>>();

        // Iterate through the asynchronously fetched data in batches.
        await foreach (var data in FetchStreamAsync(batchSize))
        {
            var responseData =  CryptoHelper.EncryptData(data, sharedKey);
            await stream.WriteAsync(responseData);
        }
            
        // Return the result of the streaming context.
        return stream.Result();
    }

    public virtual UnaryResult<List<TModel>> FindByParameters(byte[] parameters)
    {
        return ExecuteAsyncWithoutResponse(async () =>
        {
            var dictionary = parameters.UnPickleFromBytes<KeyValuePair<string, object>[]>();
 
            var whereStatement = string.Join(" AND ", dictionary.Select(x => $" {x.Key} = @{x.Key}").ToList());

            var result = await GetDatabase().QueryAsync($"SELECT * FROM {typeof(TModel).Name} WHERE {whereStatement}", dictionary);

            var returnData = result.Adapt(typeof(List<TModel>));

            return result.Adapt<List<TModel>>();

        });

    }

    public UnaryResult<EncryptedData<List<TModel>>> FindByParametersEncryptedAsync(EncryptedData<byte[]> parameterBytes)
    {
        return ExecuteAsyncWithoutResponse(async () =>
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
}

