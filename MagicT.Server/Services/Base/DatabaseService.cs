﻿using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Benutomo;
using MagicOnion;
using MagicT.Server.Managers;
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
[AutomaticDisposeImpl]
public abstract partial class DatabaseService<TService, TModel, TContext> :  MagicServerBase<TService>, IMagicService<TService, TModel>, IAsyncDisposable,IDisposable
    where TContext : DbContext 
    where TModel : class
    where TService : IMagicService<TService, TModel>, IService<TService>
{
    [EnableAutomaticDispose]
    // The database context instance used for database operations.
    protected TContext Db { get; set; }

    [EnableAutomaticDispose] 
    protected AuditManager AuditManager { get; set; }

    [EnableAutomaticDispose] 
    protected QueryManager QueryManager { get; set; }


    protected DatabaseService(IServiceProvider provider):base(provider)
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
    ///     Creates multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to create.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the created models.</returns>
    public UnaryResult<List<TModel>> CreateRangeAsync(List<TModel> models)
    {
        return ExecuteAsync(async () =>
        {
            // Add the models to the database context.
            Db.Set<TModel>().AddRange(models);

            // Save the changes to the database.
            await Db.SaveChangesAsync();
 
            // Return the created models.
            return models;
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
    ///     Updates multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to update.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the updated models.</returns>
    public UnaryResult<List<TModel>> UpdateRangeAsync(List<TModel> models)
    {
        return ExecuteAsync(async () =>
        {
            Db.SaveChanges();

            Db.Set<TModel>().UpdateRange(models);

            await Db.SaveChangesAsync();

            return models;
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
    ///     Removes multiple instances of the specified model asynchronously.
    /// </summary>
    /// <param name="models">The list of models to remove.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> indicating the success of the removal operation.</returns>
    public UnaryResult<List<TModel>> DeleteRangeAsync(List<TModel> models)
    {
        return ExecuteAsync(async () =>
        {
            Db.Set<TModel>().RemoveRange(models);

            await Db.SaveChangesAsync();

            return models;
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

            var queryResult = await Db.Manager().QueryAsync(queryData.query, queryData.parameters);

            return queryResult.Adapt<List<TModel>>();
        });
    }

    public virtual UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return ExecuteAsync(async () =>
        {
            var queryData = QueryManager.BuildQuery<TModel>(parameters);

            var result = await Db.Manager().QueryAsync(queryData.query, queryData.parameters);

            return result.Adapt<List<TModel>>();

        });
    }

    /// <summary>
    /// Streams all models in batches.
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>A <see cref="ServerStreamingResult{List{TModel}}"/> representing the streamed data.</returns>
    public virtual async Task<ServerStreamingResult<List<TModel>>> StreamReadAllAsync(int batchSize)
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
    /// OldStreaming
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>An asynchronous enumerable of batches of <typeparamref name="TModel"/>.</returns>
    //protected  async IAsyncEnumerable<List<TModel>> FetchStreamAsync(int batchSize = 10)
    //{
    //    // Get the total count of entities.
    //    var count = await Db.Set<TModel>().AsNoTracking().CountAsync().ConfigureAwait(false);

    //    // Calculate the number of batches required.
    //    var batches = (int)Math.Ceiling((double)count / batchSize);

    //    for (var i = 0; i < batches; i++)
    //    {
    //        var skip = i * batchSize;
    //        var take = Math.Min(batchSize, count - skip);

    //        // Fetch a batch of entities asynchronously.
    //        var entities = await Db.Set<TModel>().AsNoTracking().Skip(skip).Take(take).ToListAsync()
    //            .ConfigureAwait(false);

    //        //Yield the batch of entities.
    //        yield return entities;
    //    }
    //}


    /// <summary>
    /// Asynchronously fetches and yields data in batches.
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>An asynchronous enumerable of batches of <typeparamref name="TModel"/>.</returns>
    protected async IAsyncEnumerable<List<TModel>> FetchStreamAsync(int batchSize = 10)
    {
        // Get the type of the model
        var modelType = typeof(TModel);

        // Find the primary key property
        var keyProperty = modelType.GetProperties()
            .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Any());

        if (keyProperty == null)
        {
            throw new InvalidOperationException("No key property found on model.");
        }

        // Get the total count of entities
        var count = await Db.Set<TModel>().AsNoTracking().CountAsync().ConfigureAwait(false);

        // Calculate the number of batches required
        var batches = (int)Math.Ceiling((double)count / batchSize);

        var lastFetchedKey = default(object);

        for (var i = 0; i < batches; i++)
        {
            // Create the query
            var query = Db.Set<TModel>().AsNoTracking();

            // If this isn't the first batch, filter on the last fetched key
            if (lastFetchedKey != null)
            {
                var parameter = Expression.Parameter(modelType, "e");
                var property = Expression.Property(parameter, keyProperty.Name);
                var constant = Expression.Constant(lastFetchedKey);
                var comparison = Expression.GreaterThan(property, constant);
                var lambda = Expression.Lambda<Func<TModel, bool>>(comparison, parameter);

                query = query.Where(lambda);
            }

            // Fetch a batch of entities asynchronously
            var entities = await query
                .OrderBy(e => EF.Property<object>(e, keyProperty.Name))
                .Take(batchSize)
                .ToListAsync()
                .ConfigureAwait(false);

            // If no more entities are returned, exit the loop
            if (!entities.Any())
            {
                break;
            }

            // Update the last fetched key
            lastFetchedKey = keyProperty.GetValue(entities.Last());

            // Yield the batch of entities
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
