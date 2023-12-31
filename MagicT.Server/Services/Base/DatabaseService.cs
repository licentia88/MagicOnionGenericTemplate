﻿using AQueryMaker.Extensions;
using MagicOnion;
using MagicT.Server.Filters;
using MagicT.Server.Managers;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services.Base;

/// <summary>
/// Base class for database services providing common database operations.
/// </summary>
/// <typeparam name="TService">The service interface.</typeparam>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TContext">The database context type.</typeparam>
public class DatabaseService<TService, TModel, TContext> :  MagicServerBase<TService>, IMagicService<TService, TModel>
    where TContext : DbContext 
    where TModel : class
    where TService : IMagicService<TService, TModel>, IService<TService>
{
    // The database context instance used for database operations.
    protected TContext Db { get; set; }

    protected AuditManager AuditManager { get; set; }

    public QueryManager QueryManager { get; set; }

    public DatabaseService(IServiceProvider provider):base(provider)
    {
        Db = provider.GetService<TContext>();

        AuditManager = provider.GetService<AuditManager>();

        QueryManager = provider.GetService<QueryManager>();
    }

    /// <summary>
    ///     Creates a new instance of the specified model.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A unary result containing the created model.</returns>
    public virtual UnaryResult<TModel> CreateAsync(TModel model)
    {
        return ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Add(model);

            await Db.SaveChangesAsync();

            return model;
        });
    }

    /// <summary>
    ///     Retrieves all models.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    public virtual UnaryResult<List<TModel>> ReadAsync()
    {
        return ExecuteAsync(async () => await Db.Set<TModel>().AsNoTracking().ToListAsync());


    }

    /// <summary>
    ///     Updates the specified model.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A unary result containing the updated model.</returns>
    public virtual UnaryResult<TModel> UpdateAsync(TModel model)
    {
        return ExecuteAsync(async () =>
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
    public virtual UnaryResult<TModel> DeleteAsync(TModel model)
    {
        return ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Remove(model);

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
    ///     A <see cref="UnaryResult{ListTModel}" /> representing the result of the operation, containing a list of
    ///     entities.
    /// </returns>
    public virtual UnaryResult<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return ExecuteAsync(async () =>
        {
            KeyValuePair<string, object> parameter = new(foreignKey, parentId);

            var queryData = QueryManager.BuildQuery<TModel>(parameter);

            var queryResult = await Db.SqlManager().QueryAsync(queryData.query, queryData.parameters);

            return queryResult.Adapt<List<TModel>>();
        });
    }

    public virtual UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return ExecuteAsync(async () =>
        {
            var queryData = QueryManager.BuildQuery<TModel>(parameters);

            var result = await Db.SqlManager().QueryAsync(queryData.query, queryData.parameters);

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

    public virtual async UnaryResult<EncryptedData<TModel>> CreateEncrypted(EncryptedData<TModel> encryptedData)
    { 
        var decryptedData = CryptoHelper.DecryptData(encryptedData, SharedKey);

        var response = await CreateAsync(decryptedData);

        var cryptedData = CryptoHelper.EncryptData(response, SharedKey);

        return cryptedData;
    }

    public virtual async UnaryResult<EncryptedData<List<TModel>>> ReadEncryptedAsync()
    {
        var sharedKey = SharedKey;

        var response = await ReadAsync();
                    
        return CryptoHelper.EncryptData(response, sharedKey);
    }

    public virtual async UnaryResult<EncryptedData<TModel>> UpdateEncrypted(EncryptedData<TModel> encryptedData)
    { 
        byte[] _sharedSecret = null;

        var decryptedData = CryptoHelper.DecryptData(encryptedData, _sharedSecret);

        var response = await UpdateAsync(decryptedData);

        return CryptoHelper.EncryptData(response, _sharedSecret);
    }

    public virtual async UnaryResult<EncryptedData<TModel>> DeleteEncryptedAsync(EncryptedData<TModel> encryptedData)
    {
        var decryptedData = CryptoHelper.DecryptData(encryptedData, SharedKey);

        var response = await DeleteAsync(decryptedData);

        return CryptoHelper.EncryptData(response, SharedKey);
    }

    public virtual async UnaryResult<EncryptedData<List<TModel>>> FindByParentEncryptedAsync(EncryptedData<string> parentId, EncryptedData<string> foreignKey)
    {
        var decryptedKey = CryptoHelper.DecryptData(foreignKey, SharedKey);

        var decryptedId = CryptoHelper.DecryptData(parentId, SharedKey);

        var respnseData = await FindByParentAsync(decryptedId, decryptedKey);

        return CryptoHelper.EncryptData(respnseData, SharedKey);
    }

    public virtual UnaryResult<EncryptedData<List<TModel>>> FindByParametersEncryptedAsync(EncryptedData<byte[]> parameterBytes)
    {
        return ExecuteAsync(async () =>
        {
            var decryptedBytes = CryptoHelper.DecryptData(parameterBytes, SharedKey);

            var queryData = QueryManager.BuildQuery<TModel>(decryptedBytes);

            var result = await Db.SqlManager().QueryAsync(queryData.query, queryData.parameters);

            return CryptoHelper.EncryptData(result.Adapt<List<TModel>>(), SharedKey);
        });
    }

    public virtual async Task<ServerStreamingResult<EncryptedData<List<TModel>>>> StreamReadAllEncyptedAsync(int batchSize)
    {
 
        // Get the server streaming context for the list of TModel.
        var stream = GetServerStreamingContext<EncryptedData<List<TModel>>>();

        // Iterate through the asynchronously fetched data in batches.
        await foreach (var data in FetchStreamAsync(batchSize))
        {
            var responseData = CryptoHelper.EncryptData(data, SharedKey);
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
    private  async IAsyncEnumerable<List<TModel>> FetchStreamAsync(int batchSize = 10)
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

    protected void BeginTransaction()
    {
        Transaction = Db.Database.BeginTransaction();
    }

    protected async Task BeginTransactionAsync()
    {
        Transaction = await Db.Database.BeginTransactionAsync();
    }
}
