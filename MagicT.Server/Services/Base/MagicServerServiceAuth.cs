﻿using AQueryMaker.Extensions;
using MagicOnion;
using MagicT.Server.Filters;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services.Base;

[MagicTAuthorize]
public abstract class MagicServerAuthService<TService, TModel, TContext> : AuditDatabaseService<TService, TModel, TContext>, ISecureMagicService<TService, TModel>
    where TService : ISecureMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{


    // ReSharper disable once PublicConstructorInAbstractClass
    public MagicServerAuthService(IServiceProvider provider) : base(provider)
    {
    }



    public virtual async UnaryResult<EncryptedData<TModel>> CreateEncrypted(EncryptedData<TModel> encryptedData)
    {
        var decryptedData = CryptoHelper.DecryptData(encryptedData, SharedKey);

        var response = await CreateAsync(decryptedData);

        var cryptedData = CryptoHelper.EncryptData(response, SharedKey);

        return cryptedData;
    }

    public virtual async UnaryResult<EncryptedData<List<TModel>>> ReadEncrypted()
    {
        var sharedKey = SharedKey;

        var response = await ReadAsync();

        return CryptoHelper.EncryptData(response, sharedKey);
    }

    public virtual async UnaryResult<EncryptedData<TModel>> UpdateEncrypted(EncryptedData<TModel> encryptedData)
    {
 
        var decryptedData = CryptoHelper.DecryptData(encryptedData, SharedKey);

        var response = await UpdateAsync(decryptedData);

        return CryptoHelper.EncryptData(response, SharedKey);
    }

    public virtual async UnaryResult<EncryptedData<TModel>> DeleteEncrypted(EncryptedData<TModel> encryptedData)
    {
        var decryptedData = CryptoHelper.DecryptData(encryptedData, SharedKey);

        var response = await DeleteAsync(decryptedData);

        return CryptoHelper.EncryptData(response, SharedKey);
    }

    public virtual async UnaryResult<EncryptedData<List<TModel>>> FindByParentEncrypted(EncryptedData<string> parentId, EncryptedData<string> foreignKey)
    {
        var decryptedKey = CryptoHelper.DecryptData(foreignKey, SharedKey);

        var decryptedId = CryptoHelper.DecryptData(parentId, SharedKey);

        var respnseData = await FindByParentAsync(decryptedId, decryptedKey);

        return CryptoHelper.EncryptData(respnseData, SharedKey);
    }

    public virtual UnaryResult<EncryptedData<List<TModel>>> FindByParametersEncrypted(EncryptedData<byte[]> parameterBytes)
    {
        return ExecuteAsync(async () =>
        {
            var decryptedBytes = CryptoHelper.DecryptData(parameterBytes, SharedKey);

            var queryData = QueryManager.BuildQuery<TModel>(decryptedBytes);

            var result = await Db.SqlManager().QueryAsync(queryData.query, queryData.parameters);

            return CryptoHelper.EncryptData(result.Adapt<List<TModel>>(), SharedKey);
        });
    }

    public virtual async Task<ServerStreamingResult<EncryptedData<List<TModel>>>> StreamReadAllEncypted(int batchSize)
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

   

  
}
